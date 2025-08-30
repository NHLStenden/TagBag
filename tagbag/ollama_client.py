# ---------------------------------
# Ollama powered analyzers
# ---------------------------------

import ollama


from dataclasses import dataclass


@dataclass
class OllamaClient:
    """
    Minimal client for the ollama Python package.
    """
    model: str = ""

    def generate(self, prompt: str) -> str:
        """
        Single call to ollama.generate. Returns the response text or an empty string on failure.
        """
        if not self.model:
            return ""
        try:
            result = ollama.generate(model=self.model, prompt=prompt)
            # The package returns a dict with a "response" field
            return str(result.get("response", ""))
        except Exception:
            return ""