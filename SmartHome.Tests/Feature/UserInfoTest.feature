Feature: UserInfoTest
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers


Scenario: Register a user
	Given I have a new UserInfoEntity record with the following properties

		| UserInfoId | LocalId | Password | UserName |  DateOfBirth  |Email          |IsEmailRecipient  |IsSMSRecipient  |IsActive |
		| 1		     |       1 | App-1    | version-1| 1/2/2015      |asd@emaiyl.com |0                 |0               |1        |

	When I save the UserInfoEntity
	Then the UserInfoEntity is saved successfully
		And I can retrieve the UserInfoEntity by UserInfoId
