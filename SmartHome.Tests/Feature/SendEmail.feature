Feature: Send Email
	In order to send email
	As a records manager
	I want to send email

Scenario: Send Email
	Given I have a  EmailoEntity record with the following property
	| field				| value			|
	| JsonString |fySo081ThNMghoC+ykGyDfE4wnt7XMVKZye3dj0kDWfflbNf7nKA0XSljy5ZpA/3TXFLNN+87CfneGm35VBpgbxp0uLD7PlyZ5xFnYVnx3c=|
	When I send the Email
	Then I will check Email sent or not and response  

