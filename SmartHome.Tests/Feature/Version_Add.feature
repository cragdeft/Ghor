Feature: Version_Add
	In order track version records
	As a records manager
	I need to add new versions

Scenario: Add a version
	Given I have a new version record with the following properties
		| id | VersionId | AppName | AppVersion | AuthCode |
		| 1  |          1 | App-1   | version-1  | 0123456789ABCDEF |		
	When I save the version
	Then the version is saved successfully
		And I can retrieve the version by Id
