from .tag_base import TagBase
from .tag_helper import TagHelper


from pathlib import Path
from typing import Dict, Iterable, Sequence


class TagCounter(TagBase):
    """
    Count frequency of tags across all files and print counts to stdout.
    """

    @staticmethod
    def execute(args: Sequence[str]) -> None:
        """
        args[0]: directory path
        """
        file_names = TagBase.get_tag_files(args[0])
        tokens: Dict[str, int] = {}
        TagCounter._count_tags_in_files(file_names, tokens)

        for token, count in sorted(tokens.items(), key=lambda kv: kv[1], reverse=True):
            print(f"{token}: {count}")

    @staticmethod
    def _count_tags_in_files(file_names: Iterable[str], tokens: Dict[str, int]) -> None:
        for name in file_names:
            TagCounter._count_tags_in_file(Path(name), tokens)

    @staticmethod
    def _count_tags_in_file(file_path: Path, tokens: Dict[str, int]) -> None:
        for line in file_path.read_text(encoding="utf-8").splitlines():
            for item in TagHelper.split_into_tags(line):
                tag = item.strip()
                if not tag:
                    continue
                tokens[tag] = tokens.get(tag, 0) + 1