from .ollama_client import OllamaClient
from .tag_base import TagBase

from pathlib import Path
from typing import Iterable, Sequence


class TagTransformer(TagBase):
    """
    Transform each file through an LLM and overwrite it with the response text.
    """

    @staticmethod
    def execute(args: Sequence[str]) -> None:
        """
        args[0]: directory path
        args[1] optional: model name
        args[2] optional: prompt file name
        """
        dir_path = args[0]
        model = args[1] if len(args) >= 2 else ""
        prompt_file = args[2] if len(args) >= 3 else ""

        file_names = TagBase.get_tag_files(dir_path)
        TagTransformer._transform_files(file_names, model, prompt_file)

    @staticmethod
    def _transform_files(file_names: Iterable[str], model: str, prompt_file_name: str) -> None:
        for name in file_names:
            TagTransformer._transform_file(Path(name), model, prompt_file_name)

    @staticmethod
    def _transform_file(file_path: Path, model: str, prompt_file_name: str) -> None:
        client = OllamaClient(model=model)
        try:
            body = "\n".join(file_path.read_text(encoding="utf-8").splitlines())
            prompt_prefix = Path(prompt_file_name).read_text(encoding="utf-8") if prompt_file_name else ""
            prompt = f"{prompt_prefix}{body}"
            response_text = client.generate(prompt)
            if response_text:
                file_path.write_text(response_text, encoding="utf-8")
        except FileNotFoundError:
            # Missing file or prompt file. Skip this path.
            pass