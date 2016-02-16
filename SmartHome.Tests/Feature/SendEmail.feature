﻿Feature: Send Email
	In order to send email
	As a records manager
	I want to send email

Scenario: Send Plain Email
	Given I have a  EmailoEntity record with the following properties

		| FromAddress                     | ToAddress         | Subject                | Body | EnableSSL | IsBodyHtml | SentDate |
		| smarthome-noreply@sinepulse.com | sumon.kumar@aplombtechbd.com | this is a test message | true | true      | true       | 1/2/2015 |
		
		When I send the Email
		Then I will check Email sent or not and response  
