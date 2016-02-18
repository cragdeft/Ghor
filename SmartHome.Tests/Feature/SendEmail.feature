Feature: Send Email
	In order to send email
	As a records manager
	I want to send email

Scenario: Send Email
	Given I have a  EmailoEntity record with the following property
	| field				| value			|
	| JsonString | { "to": "skd9000@gmail.com", "body": "this is a test", "subject": "test"}|
	Then I will encrypt the string
	When I send the Email
	Then I will check Email sent or not and response  