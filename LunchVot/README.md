## LunchVot 
LunchVot is a sample bot app designed to work with Slack.

LunchVot allows teams using Slack to vote for a location to go to lunch. Each user can vote for a place, and then the results are summarized so a decision can be made.

Here is the bot's usage:
Vote - add your vote to a location for lunch.
Ex. 'lunchvot vote Cafe1'
Unvote - Remove your vote from the current round (so you can vote again if you'd like).
Ex. 'lunchvot unvote'
List - show the current results.
Ex. 'lunchvot list'
Reset - show the results and then reset all votes, effectively starting a new round.
Ex. 'lunchvot reset'

## Focus
This sample demonstrates how simple it is to create a stateful service over the Distribyte platform. Despite not "writing" data to any durable location, this sample creates a stateful service by storing its state in variables.
This sample's state is resilient to failures and long-living, just as though it were saved in a database or another datastore, without the complexity or performance penalties.
