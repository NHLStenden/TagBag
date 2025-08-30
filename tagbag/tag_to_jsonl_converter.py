from .tag_base import TagBase
from .tag_helper import TagHelper


import json
from pathlib import Path
from typing import List, Sequence


class TagToJsonlConverter(TagBase):
    """
    Convert tag files to a JSONL file with fields: image and prompt.
    """

    @staticmethod
    def execute(args: Sequence[str]) -> None:
        """
        args[0]: directory path
        Writes train.jsonl to that directory.
        """
        dir_path = Path(args[0])
        lines: List[str] = []
        for name in TagBase.get_tag_files(dir_path):
            line = TagToJsonlConverter._process_file(Path(name))
            if line:
                lines.append(line)
        (dir_path / "train.jsonl").write_text("\n".join(lines), encoding="utf-8")

    @staticmethod
    def _process_file(file_path: Path) -> str:
        content_lines = file_path.read_text(encoding="utf-8").splitlines()
        if not content_lines:
            return ""
        image_file = TagToJsonlConverter._get_image_file_name(file_path)
        if not image_file:
            return ""
        prompt = " ".join(content_lines)
        obj = {"image": image_file, "prompt": prompt}
        return json.dumps(obj, ensure_ascii=False)

    @staticmethod
    def _get_image_file_name(text_file_path: Path) -> str:
        """
        Find a file in the same directory with the same stem but a different extension.
        Returns empty string if not found.
        """
        directory = text_file_path.parent
        stem = text_file_path.stem
        for f in directory.iterdir():
            if f.stem == stem and f.resolve() != text_file_path.resolve():
                return str(f)
        return ""