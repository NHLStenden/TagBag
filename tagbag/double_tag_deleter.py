# ---------------------------------
# File processors
# ---------------------------------

from .tag_base import TagBase
from .tag_helper import TagHelper


from pathlib import Path
from typing import List, Sequence, Set


class DoubleTagDeleter(TagBase):
    """
    Remove duplicate tags within each line of every file.
    """

    @staticmethod
    def execute(args: Sequence[str]) -> None:
        """
        args[0]: directory path
        """
        file_names = TagBase.get_tag_files(args[0])
        for name in file_names:
            DoubleTagDeleter._process_file(Path(name))

    @staticmethod
    def _process_file(file_path: Path) -> None:
        lines = file_path.read_text(encoding="utf-8").splitlines()
        new_lines: List[str] = []

        for line in lines:
            items = [t.strip() for t in TagHelper.split_into_tags(line) if t.strip()]
            seen: Set[str] = set()
            deduped = [t for t in items if not (t in seen or seen.add(t))]
            new_lines.append(", ".join(deduped))

        file_path.write_text("\n".join(new_lines), encoding="utf-8")