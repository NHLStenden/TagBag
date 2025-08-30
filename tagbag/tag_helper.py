# ---------------------------------
# Utilities and shared base
# ---------------------------------

from typing import List


class TagHelper:
    """
    Helper functions for working with comma separated tags.
    """

    @staticmethod
    def split_into_tags(text: str) -> List[str]:
        """
        Split a single line of comma separated tags into a list of stripped items.
        Empty segments are preserved for callers that want to handle them explicitly.
        """
        return [part.strip() for part in text.split(",")]