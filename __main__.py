from __future__ import annotations

from typing import Dict, Sequence, Callable
import argparse

from tagbag import (
    DoubleTagDeleter,
    TagAnalyzer,
    TagCounter,
    TagDeleter,
    TagGrouper,
    TagToJsonlConverter,
    TagTransformer,
)

def main(argv: Sequence[str] | None = None) -> None:
    """
    Command dispatch:
      count, delete, delete-doubles, convert-to-jsonl, group, transform, analyze
    First positional argument is the command. The remaining are forwarded to the handler.
    """
    parser = argparse.ArgumentParser(prog="tagbag", add_help=True)
    parser.add_argument("command", help="count | delete | delete-doubles | convert-to-jsonl | group | transform | analyze")
    parser.add_argument("args", nargs=argparse.REMAINDER, help="Arguments passed to the selected command")
    ns = parser.parse_args(argv)

    dispatch: Dict[str, Callable[[Sequence[str]], None]] = {
        "count": TagCounter.execute,
        "delete": TagDeleter.execute,
        "delete-doubles": DoubleTagDeleter.execute,
        "convert-to-jsonl": TagToJsonlConverter.execute,
        "group": TagGrouper.execute,
        "transform": TagTransformer.execute,
        "analyze": TagAnalyzer.execute,
    }

    action = dispatch.get(ns.command)
    if not action:
        return

    if len(ns.args) < 1:
        return

    action(ns.args)


if __name__ == "__main__":
    main()
