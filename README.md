The goal of this test is to create a method of examining a Social Network. You are given dataset (data.json) representing a group of people, in the form of a social graph. Each person listed has one or more connections to the group.

Come up with a database structure to store the information found in data.json. You should then create an API, which provides functionality to choose a person within the group stored in the database and display the following information about this person:

-Direct friends: those people who are directly connected to the chosen user (required);
-Friends of friends: those who are two steps away from the chosen user but not directly connected to the chosen user (optional);
-Suggested friends: people in the group who know 2 or more direct friends of the chosen user but are not directly connected to the chosen user (optional);
