from pathlib import Path
from typing import Sequence
import json

from .tag_base import TagBase


class JsonlToTagConverter(TagBase):
    """
    Convert a JSONL file with fields: image and prompt
    back into .txt tag files next to the images.
    """

    @staticmethod
    def execute(args: Sequence[str]) -> None:
        if not args:
            raise ValueError("Path to JSONL file is required")

        jsonl_path = Path(args[0]).resolve()
        if not jsonl_path.is_file():
            raise ValueError(f"Not a file: {jsonl_path}")

        for line_number, line in enumerate(
            jsonl_path.read_text(encoding="utf-8", errors="ignore").splitlines(), start=1
        ):
            line = line.strip()
            if not line:
                continue

            try:
                obj = json.loads(line)
            except json.JSONDecodeError:
                # JSONL rule: skip bad lines, do not abort
                continue

            JsonlToTagConverter._process_entry(obj)

    @staticmethod
    def _process_entry(obj: dict) -> None:
        image_value = obj.get("image")
        prompt = obj.get("prompt")
        if not isinstance(image_value, str) or not isinstance(prompt, str):
            return

        image_path = Path(image_value)
        if not image_path.is_file():
            return

        tag_file = image_path.with_suffix(".txt")

        # Preserve prompt exactly as stored
        tag_file.write_text(prompt, encoding="utf-8")