# EasyHelpdesk
A Unity script that makes it easy to add helpdesk messages into a game. 

Add the script into your project and call EasyHelpdesk.QueueMessage(string message, float time) to display your message for the desired number of seconds. 

If you send it a message while another message is already playing, the message gets added to a queue and played in the order it was recieved.

Allows you to link your own text object to the helpdesk if you want to make it look fancy, but it'll make it's own damn text object if you don't.


