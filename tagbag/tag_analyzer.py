from .tag_base import TagBase
from .ollama_client import OllamaClient


from pathlib import Path
from typing import Iterable, List, Sequence


class TagAnalyzer(TagBase):
    """
    Analyze all tags across files using an LLM prompt.
    Prints the model output to stdout.
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

        content: List[str] = []
        TagAnalyzer._load_tags_from_files(file_names, content)

        print("Tags in files:")
        for c in content:
            print(c)

        summary = TagAnalyzer._analyze(content, model, prompt_file)
        for line in summary.splitlines():
            print(line)

    @staticmethod
    def _load_tags_from_files(file_names: Iterable[str], tokens: List[str]) -> None:
        for name in file_names:
            tokens.extend(Path(name).read_text(encoding="utf-8").splitlines())

    @staticmethod
    def _analyze(content: List[str], model: str, prompt_file_name: str) -> str:
        client = OllamaClient(model=model)
        try:
            lines = "\n".join(content)
            prompt_prefix = Path(prompt_file_name).read_text(encoding="utf-8") if prompt_file_name else ""
            prompt = f"{prompt_prefix}{lines}"
            return client.generate(prompt)
        except FileNotFoundError:
            return ""