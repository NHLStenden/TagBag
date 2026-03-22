from .ollama_client import OllamaClient
from .tag_analyzer import TagAnalyzer
from .tag_base import TagBase

from pathlib import Path
from typing import Iterable, List, Sequence


class TagAnalyzerTransformer(TagBase):
    """
    Analyze all tag files first, then transform each file using the analysis output
    as additional context for the transformation prompt.
    """

    @staticmethod
    def execute(args: Sequence[str]) -> None:
        """
        args[0]: directory path
        args[1] optional: model name
        args[2] optional: analysis prompt file name
        args[3] optional: transform prompt file name
        """
        dir_path = args[0]
        model = args[1] if len(args) >= 2 else ""
        analysis_prompt_file = args[2] if len(args) >= 3 else ""
        transform_prompt_file = args[3] if len(args) >= 4 else ""

        file_names = TagBase.get_tag_files(dir_path)

        content: list[str] = []
        TagAnalyzerTransformer._load_tags_from_files(file_names, content)

        analysis = TagAnalyzerTransformer._analyze(content, model, analysis_prompt_file)
        if not analysis:
            return

        TagAnalyzerTransformer._transform_files(
            file_names=file_names,
            model=model,
            prompt_file_name=transform_prompt_file,
            analysis=analysis,
        )

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
            
    @staticmethod
    def _transform_files(
        file_names: Iterable[str],
        model: str,
        prompt_file_name: str,
        analysis: str,
    ) -> None:
        for name in file_names:
            TagAnalyzerTransformer._transform_file(
                file_path=Path(name),
                model=model,
                prompt_file_name=prompt_file_name,
                analysis=analysis,
            )

    @staticmethod
    def _transform_file(
        file_path: Path,
        model: str,
        prompt_file_name: str,
        analysis: str,
    ) -> None:
        client = OllamaClient(model=model)
        try:
            body = file_path.read_text(encoding="utf-8")
            prompt_prefix = (
                Path(prompt_file_name).read_text(encoding="utf-8")
                if prompt_file_name
                else ""
            )

            prompt = (
                f"{prompt_prefix}\n"
                f"Global analysis:\n{analysis}\n\n"
                f"File content:\n{body}"
            )

            response_text = client.generate(prompt)
            if response_text:
                file_path.write_text(response_text, encoding="utf-8")
        except FileNotFoundError:
            # Missing file or prompt file. Skip this path.
            pass