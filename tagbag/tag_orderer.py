from .tag_base import TagBase
from .tag_helper import TagHelper


from pathlib import Path
from typing import Dict, Iterable, List, Sequence, Tuple


class TagOrderer(TagBase):
    """
    Order tags per line using a global position score learned from all files.

    For each item at index j among n items on a line, the contribution is j * (j / n).
    The running average is updated across all lines.
    """

    @staticmethod
    def execute(args: Sequence[str]) -> None:
        """
        args[0]: directory path
        """
        file_names = TagBase.get_tag_files(args[0])
        tokens: Dict[str, Tuple[int, float]] = {}
        TagOrderer._count_tags_in_files(file_names, tokens)
        TagOrderer._order(file_names, tokens)

    @staticmethod
    def _count_tags_in_files(file_names: Iterable[str], tokens: Dict[str, Tuple[int, float]]) -> None:
        for name in file_names:
            TagOrderer._count_tags_in_file(Path(name), tokens)

    @staticmethod
    def _count_tags_in_file(file_path: Path, tokens: Dict[str, Tuple[int, float]]) -> None:
        for line in file_path.read_text(encoding="utf-8").splitlines():
            items = [t.strip() for t in TagHelper.split_into_tags(line) if t.strip()]
            n = len(items)
            if n == 0:
                continue
            for j, item in enumerate(items):
                current_position = j * (j / float(n))
                if item in tokens:
                    count, avg_pos = tokens[item]
                    new_count = count + 1
                    new_pos = (current_position / new_count) + (avg_pos * ((new_count - 1) / float(new_count)))
                    tokens[item] = (new_count, new_pos)
                else:
                    tokens[item] = (1, current_position)

    @staticmethod
    def _order(file_names: Iterable[str], tokens: Dict[str, Tuple[int, float]]) -> None:
        for name in file_names:
            p = Path(name)
            lines = p.read_text(encoding="utf-8").splitlines()
            out_lines: List[str] = []

            for line in lines:
                items = [t.strip() for t in TagHelper.split_into_tags(line) if t.strip()]
                scored: List[Tuple[str, float]] = []
                for item in items:
                    if item in tokens:
                        scored.append((item, tokens[item][1]))
                scored.sort(key=lambda x: x[1])
                out_lines.append(", ".join(token for token, _ in scored))

            p.write_text("\n".join(out_lines), encoding="utf-8")