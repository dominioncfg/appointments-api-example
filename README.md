This Project requires .Net 8 SDK


List Of Assumptions:

1. Given that the external api does not support passing the Facility Id to give you different schedules and always seem to return the same one, I've built the application in the same manner, (meaning is not a multi-tenant app)
2. The request of taking a slot when I follow the documentation seems to be missing the Facility Id, I decided to add (In working conditions this is better asked on the team , but I was very limited on time).
3. What about weekends? I could not find any documentation in relation to whether or not doctors are allowed to work  on weekends, so I decided to assume that is possible. (In working conditions this is better asked on the team , but I was very limited on time).
