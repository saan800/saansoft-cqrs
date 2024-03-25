# saansoft-cqrs

[![build-and-test](https://github.com/saan800/saansoft-cqrs/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/saan800/saansoft-cqrs/actions/workflows/ci.yml)

CQRS and Event Sourcing implementation for c#


## Setup

### Git Hooks

Run the following to enable git hooks

```shell
git config core.hooksPath .githooks
```

All PRs will run the same scripts as part of the CI pipeline.


### Spelling

The `dictionary.dic` file at the root level of the repository contains project specific words.

#### VS Code

Add the [Code Spell Checker](https://marketplace.visualstudio.com/items?itemName=streetsidesoftware.code-spell-checker) plugin and enable it.

The `.cspell.json` file in the root directory is configured to use and add any unknown words to the dictionary.


#### Jet Brains Rider

Open the project then go to:

* `Settings` > `Editor` > `Spelling`
* Check `Use single dictionary for saving words` and select `application-level`
* Under `Custom dictionaries (plain text word lists, hunspell)`
  * Click the `+` button
  * In the root directory of the repository, select `dictionary.dic`
  * Save the changes


