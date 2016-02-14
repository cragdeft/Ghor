Feature: ParseCommand for Feedback
	In order to parse commands
	As a records manager
	I want to Parse Command Json

	
Scenario: Parse CommandJson for OnOffFeedback
	Given I have entered following property  
	| field				| value			|
	| JsonString | { "response": true, "device_version": "00", "email": "hvuv@vuu.com", "device_uuid": 2094027172,"device_id": 32769, "mac_id": "mac","command_byte": "[1, 2, 1, 1, 1, 255, 255, -5]","command_id": 58} |
	And I created a json with that string
	When I parse
	Then I will check onoff status

Scenario: Parse CommandJson for ThermalShutDown
	Given I have entered following property  
	| field				| value			|
	| JsonString | { "response": true, "device_version": "00", "email": "hvuv@vuu.com", "device_uuid": 2094027172,"device_id": 32769, "mac_id": "mac","command_byte": "[1, 56, 1, 1, 1, 255, 255, -5]","command_id": 56} |
	And I created a json with that string
	When I parse
	Then I will check thermalshutdown status

Scenario: Parse CommandJson for DemmingEnableDisable
	Given I have entered following property  
	| field				| value			|
	| JsonString | { "response": true, "device_version": "00", "email": "hvuv@vuu.com", "device_uuid": 2094027172,"device_id": 32769, "mac_id": "mac","command_byte": "[1, 64, 1, 1, 1, 255, 255, -5]","command_id": 56} |
	And I created a json with that string
	When I parse
	Then I will check dimmingEnableDisable status


Scenario: Parse CommandJson for LoadType
	Given I have entered following property  
	| field				| value			|
	| JsonString | { "response": true, "device_version": "00", "email": "hvuv@vuu.com", "device_uuid": 2094027172,"device_id": 32769, "mac_id": "mac","command_byte": "[1, 58, 0, 1, 1, 255, 255, -5]","command_id": 58} |
	And I created a json with that string
	When I parse
	Then I will check LoadType status

Scenario: Parse CommandJson for CurrentLoadStatus32Byte
	Given I have entered following property  
	| field				| value			|
	| JsonString | { "response": true, "device_version": "00", "email": "hvuv@vuu.com", "device_uuid": 2094027172,"device_id": 32769, "mac_id": "mac","command_byte": "[1,6,32, 1, 0, 2, 2, 2, 1, 2, 2, 2,2, 2, 2, 2,3,2, 2, 2,4, 2, 2, 2,5, 2, 2, 2,6, 2, 2, 2]","command_id": 6} |
	And I created a json with that string
	When I parse
	Then I will check CurrentLoadStatusThreeTwoByte status

Scenario: Parse CommandJson for CurrentLoadStatus8Byte
	Given I have entered following property  
	| field				| value			|
	| JsonString | { "response": true, "device_version": "00", "email": "hvuv@vuu.com", "device_uuid": 2094027172,"device_id": 32769, "mac_id": "mac","command_byte": "[1, 6, 8, 1, 1, 2, 2, 2]","command_id": 6} |
	And I created a json with that string
	When I parse
	Then I will check CurrentLoadStatusEightByte status

	Scenario: Parse CommandJson for DimmingFeedback
	Given I have entered following property  
	| field				| value			|
	| JsonString | { "response": true, "device_version": "00", "email": "hvuv@vuu.com", "device_uuid": 2094027172,"device_id": 32769, "mac_id": "mac","command_byte": "[1, 52, 1, 1, 1, 255, 255, -5]","command_id": 52} |
	And I created a json with that string
	When I parse
	Then I will check DimmingFeedback status

Scenario: Parse CommandJson for RgbwSetFeedback
	Given I have entered following property  
	| field				| value			|
	| JsonString | { "response": true, "device_version": "00", "email": "hvuv@vuu.com", "device_uuid": 2094027173,"device_id": 32767, "mac_id": "mac","command_byte": "[1, 99, 1, 1, 1, 0, 1, -5]","command_id": 99} |
	And I created a json with that string
	When I parse
	Then I will check RgbwSetFeedback status

Scenario: Parse CommandJson for RainbowOnOffFeedback
	Given I have entered following property  
	| field				| value			|
	| JsonString | { "response": true, "device_version": "00", "email": "hvuv@vuu.com", "device_uuid": 2094027173,"device_id": 32767, "mac_id": "mac","command_byte": "[1, 97, 1, 1, 1, 0, 1, -5]","command_id": 97} |
	And I created a json with that string
	When I parse
	Then I will check RainbowOnOffFeedback status
