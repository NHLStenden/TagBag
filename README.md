
# TagBag
TagBag is a Python command-line tool for processing tag-based text files. It supports various operations such as counting, deleting, reordering, transforming, grouping, and converting tag data — useful in annotation workflows and general-purpose tag management.

## Features
- Works with .txt and .npz files
- Count tag frequency
- Delete specific tags or duplicates
- Reorder tags using a mean-based method
- Convert tags to JSONL format
- Group or transform tags
- Analyze tags

## Requirements
- Python (3.12 or later)
- Ollama (for the transform feature)

### Install
```bash
git clone https://github.com/NHLStenden/TagBag
cd TagBag
python -m venv .venv
# macOS or Linux
source .venv/bin/activate
# Windows PowerShell
.venv\Scripts\Activate.ps1
pip install -r requirements.txt
```

### Run
Use the module entry point:
```bash
python main.py <command> [arguments]
```

### Commands

#### count
Counts tag occurrences in all files at the given path.

Arguments:
1. Path to .txt or .npz files

```bash
python main.py count data/
```

#### delete
Deletes specific tags from all files at the given path.

Arguments:
1. Path to .txt or .npz files
2. Text file with tags to remove, one per line

```bash
python main.py delete data/ remove_these_tags.txt
```

#### delete-doubles
Removes duplicate tags inside lines across all files at the given path.

Arguments:
1. Path to .txt or .npz files

```bash
python main.py delete-doubles data/
```

#### convert-to-jsonl
Converts a dataset folder to JSONL with fields image and prompt.

Arguments:
1. Path to dataset that contains images and .txt or .npz files

```bash
python main.py convert-to-jsonl data/
# Output: data.jsonl in the current directory unless otherwise configured
```

Sample JSONL line:
```json
{"image": "data/images/000123.jpg", "prompt": "cat, sitting, window light"}
```

#### group
Regroups specified tag groups inside lines.

Arguments:
1. Path to .txt or .npz files
2. One or more group specs in quotes. Each spec is a comma separated list of tags that belong together

```bash
# Example: treat these as the same group inside lines
python main.py group data/ "car, auto, vehicle" "cat, kitten"
```

#### transform
Transforms each file through an LLM and overwrites it with the response text. Requires Ollama.

Arguments:
1. Directory path of .txt or .npz files
2. Ollama model name
3. Prompt file name

```bash
# Make sure Ollama is running locally
python main.py transform data/ llama3.1 prompts/normalize.txt
```

#### analyze
Analyzes all tags across files using an LLM prompt. Prints the model output to stdout. Requires Ollama.

Arguments:
1. Directory path of .txt or .npz files
2. Ollama model name
3. Prompt file name

```bash
python main.py analyze data/ llama3.1 prompts/analysis.txt
```

# Contributing
Contributions are welcome! Please fork the repository and submit pull requests.

# License
This project is licensed under the MIT License.

# Acknowledgements
NHL Stenden: For providing the foundational code and utilities.
Martin Bosgra: Author and primary maintainer of the project.
