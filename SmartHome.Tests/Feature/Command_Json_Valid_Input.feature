Feature: Command_Json_Valid_Input
	In order to avoid silly mistakes
	I want to check json parse process with varity of input set

Scenario Outline: Command json parse check
    Given I'm using  command json parameter
    When User enter '<response>' and '<email>'
    Then the parser check my status

    Examples:
        | response | email       | device_uuid | device_id | mac_id | command_byte | AppID |
        | true     | a@yahoo.com | 80          | 1         | 0      | 1,1,1        | 10    |
		| false     | a@yahoo.com | 80          | 1         | 0      | 1,1,1        | 10    |
		| false     | a@yahoo.com | 80          | 1         | 0      | 1,1,1        | 10    |
		| true     | a@yahoo.com | 80          | 1         | 0      | 1,1,1        | 10    |
        
        
