# Samples


## Xamarin CRM Sample
This is a modified version of the Xamarin CRM sample available from their website. It has been modified to take advantage of Distribyte Platform's statefulness, and no longer requires a database.
These changes significantly simplify the sample's code and make it much easier to understand.

It can also be run locally in a way that's identical to how it runs in the cloud with no additional dependencies.

## LunchVot 
LunchVot is a sample bot app designed to work with Slack.

LunchVot allows teams using Slack to vote for a location to go to lunch. Each user can vote for a place, and then the results are summarized so a decision can be made.
### Focus
This sample demonstrates how simple it is to create a stateful service over the Distribyte platform. Despite not "writing" data to any durable location, this sample creates a stateful service by storing its state in variables.

This sample's state is resilient to failures and is long-living, just as though it were saved in a database or another datastore, without the complexity or performance penalties.
