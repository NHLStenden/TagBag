from .tag_base import TagBase
from .tag_helper import TagHelper


from pathlib import Path
from typing import List, Sequence, Set


class TagDeleter(TagBase):
    """
    Delete specified tags (listed one per line in a separate file) from each line.
    """

    @staticmethod
    def execute(args: Sequence[str]) -> None:
        """
        args[0]: directory path
        args[1] optional: text file containing tags to remove, one per line
        """
        dir_path = args[0]
        remove_words_file = args[1] if len(args) >= 2 else ""
        remove_words: List[str] = []
        if remove_words_file:
            remove_words = Path(remove_words_file).read_text(encoding="utf-8").splitlines()

        file_names = TagBase.get_tag_files(dir_path)
        remove_set = {w.strip() for w in remove_words if w.strip()}
        for name in file_names:
            TagDeleter._process_file(Path(name), remove_set)

    @staticmethod
    def _process_file(file_path: Path, remove_words: Set[str]) -> None:
        lines = file_path.read_text(encoding="utf-8").splitlines()
        out_lines: List[str] = []

        for line in lines:
            kept: List[str] = []
            for item in TagHelper.split_into_tags(line):
                tag = item.strip()
                if tag and tag not in remove_words:
                    kept.append(tag)
            out_lines.append(", ".join(kept))

        file_path.write_text("\n".join(out_lines), encoding="utf-8")