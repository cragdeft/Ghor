Feature: Configuration_Json_Valid_Input
	In order to avoid silly mistakes
	I want to check json parse process with varity of input set

Scenario: Configuration json with valid input
	Given I am at the configuration json input page
	When I fill in the following form
	| field				| value			|
	| MessgeTopic		| Configuration |
	| PublishMessage	| {"Version":[{"Id":1,"AppName":"SmartHome","AppVersion":"1.5","AuthCode":"0123456789ABCDEF","PassPhrase":"Y1JJ9N"}],"VersionDetails":[{"Id":1,"VersionId":1,"HardwareVersion":"00","DeviceType":1},{"Id":2,"VersionId":1,"HardwareVersion":"00","DeviceType":2}],"Device":[{"Id":1,"DeviceId":32769,"DeviceHash":1606113433,"DeviceType":0,"DeviceName":"SMSW6G 1606113433","Version":"00","IsDeleted":false,"Watt":0}],"DeviceStatus":[{"Id":1,"DeviceTableId":1,"StatusType":53,"StatusValue":"1"},{"Id":2,"DeviceTableId":1,"StatusType":5,"StatusValue":"1"}],"Channel":[{"Id":1,"DeviceTableId":1,"ChannelNo":2,"LoadType":3,"LoadName":"Fan","LoadWatt":0},{"Id":2,"DeviceTableId":1,"ChannelNo":6,"LoadType":5,"LoadName":"CFL","LoadWatt":0}],"NextAssociatedDeviceId":[{"NextDeviceId":32770}],"ChannelStatus":[{"Id":1,"ChannelTableId":1,"StatusType":1,"StatusValue":"1"},{"Id":2,"ChannelTableId":1,"StatusType":3,"StatusValue":"0"},{"Id":3,"ChannelTableId":1,"StatusType":2,"StatusValue":"53"},{"Id":4,"ChannelTableId":2,"StatusType":1,"StatusValue":"0"},{"Id":5,"ChannelTableId":2,"StatusType":3,"StatusValue":"0"},{"Id":6,"ChannelTableId":2,"StatusType":2,"StatusValue":"100"}]} |
		And I click the 'publish' button for publish
		And I click the 'subscribe' button for subscrib
	Then I should be at the subscribe page