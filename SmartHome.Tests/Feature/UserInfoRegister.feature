Feature: UserInfoTest
	In order track userinfo records
	As a records manager
	I need to add new UserInfo


Scenario: Register a user
	Given I have a new UserInfoEntity record with the following properties

		| UserInfoId | LocalId | Password | UserName |  DateOfBirth  |Email          |IsEmailRecipient  |IsSMSRecipient  |IsActive |
		| 1		     |       1 | App-1    | version-1| 1/2/2015      |asd@emaiyl.com |0                 |0               |1        |

	When I save the UserInfoEntity
	Then the UserInfoEntity is saved successfully
		And I can retrieve the UserInfoEntity by UserInfoId
