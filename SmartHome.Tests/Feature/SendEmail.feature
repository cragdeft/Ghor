Feature: Send Email
	In order to send email
	As a records manager
	I want to send email

Scenario: Send Email
	Given I have a  EmailoEntity record with the following properties

		| FromAddress                     | ToAddress         | Subject                | Body | EnableSSL | IsBodyHtml | SentDate |
		| smarthome-noreply@sinepulse.com | skd9000@gmail.com | this is a test message | true | true      | true       | 1/2/2015 |
		
		When I save the Email
		Then I will check Email sent or not  

