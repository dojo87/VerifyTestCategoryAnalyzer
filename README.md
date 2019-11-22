# VerifyTestCategoryAnalyzer

An analyzer that forces a practice of assigning a test category for each test.
Need for this arose when more types of tests were introduced into a project and executing unit tests step had a big filter on it:
`TestCategory!=System&TestCategory!=Integration…` etc.
We've required having an explicit category so that no test is left out as uncategorized.

Analyzer tested for NUnit, XUnit and MSTest frameworks.

## Possible enhancements
### Possibility of specyfing a default category attribute
For XUnit, where a trait requires just to implement ITraitAttribute, one might use a custom attribute like `[Category("category")]`.
### Possibility of specyfing a default category
Currently the category is "UnitTest"
### Possibility of specyfing the category inline
Give the user a possibility of typing in the category in the editor. 
