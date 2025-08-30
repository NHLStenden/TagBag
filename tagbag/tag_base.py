from pathlib import Path
from typing import List

class TagBase:
    """
    Base class with shared file discovery.
    """

    @staticmethod
    def get_tag_files(path: str | Path) -> List[str]:
        """
        Recursively return tag files under path. First try *.txt. If none are found, try *.npz.
        """
        p = Path(path)
        try:
            txt = [str(f) for f in p.rglob("*.txt")]
            if txt:
                return txt
            npz = [str(f) for f in p.rglob("*.npz")]
            return npz
        except FileNotFoundError:
            return []