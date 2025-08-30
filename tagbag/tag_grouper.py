from .tag_base import TagBase
from .tag_helper import TagHelper


from pathlib import Path
from typing import List, Sequence


class TagGrouper(TagBase):
    """
    Regroup specified tag groups inside lines.

    Example:
      groups: ["a, b, c", "x, y"]
      line:   "c, a, b, z, y, x"
      result: "... a, b, c ..." and "... x, y ..." when all group elements are present
    """

    @staticmethod
    def execute(args: Sequence[str]) -> None:
        """
        args[0]: directory path
        args[1:]: group strings of comma separated tags
        """
        dir_path = args[0]
        groups = list(args[1:]) if len(args) >= 2 else []

        for name in TagBase.get_tag_files(dir_path):
            TagGrouper._process_file(Path(name), groups)

    @staticmethod
    def _process_file(file_path: Path, groups: List[str]) -> None:
        lines = file_path.read_text(encoding="utf-8").splitlines()
        out_lines: List[str] = []

        for line in lines:
            items = [t.strip() for t in TagHelper.split_into_tags(line)]
            cur = list(items)

            for group in groups:
                group_items = [t.strip() for t in TagHelper.split_into_tags(group)]
                indices = TagGrouper._indices_of(cur, group_items)
                indices_sorted_desc = sorted(indices, reverse=True)
                if TagGrouper._valid_indices(indices_sorted_desc):
                    for idx in indices_sorted_desc:
                        cur.pop(idx)
                    insert_at = min(indices_sorted_desc)
                    cur[insert_at:insert_at] = group_items

            out_lines.append(", ".join(cur))

        file_path.write_text("\n".join(out_lines), encoding="utf-8")

    @staticmethod
    def _indices_of(haystack: List[str], needles: List[str]) -> List[int]:
        """
        Return the indices of each needle (first occurrence) in haystack.
        Missing needles give -1 which invalidates the group.
        """
        return [haystack.index(n) if n in haystack else -1 for n in needles]

    @staticmethod
    def _valid_indices(indices: List[int]) -> bool:
        return all(i >= 0 for i in indices)