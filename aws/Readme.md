# Intro

I'm interested in local versions of the AWS services to use when developing. Let's use the docker image of the DynamoDB to develop locally using both typescript and .net core to access it. 

# Docker for DynamoDB image

```
docker run -p 8000:8000 amazon/dynamodb-local
```

# Setup the aws CLI 

First I had to install the command line interface from 
```
https://aws.amazon.com/cli/
```

# Pluralsight course on DynamoDB

Item -> Row
Attribute -> Column
Table -> Database

{ 
    "keys" : values,
},
{...} 

Partition keys are the index attributes (and partition keys)
Sort keys are secondary indexing attribues 

Tables have a Single primary keys or Composite primary key (partition + sort key)

## Datatypes
    - Scalar 
        string, integer, float, boolean, "binary" as text (base64)
    - Document
        json 
    - Set 
        sets of similar attributes

## WCU & RCU

WCU Write capacity units (1 write operation per second of 1KB)
RCU Read capacity units (1-2 read operations per second of 4KB)
    - Strongly consistent reads are required when you query directly after updating (http 200)
    
Read capacity (4 KB), per second reads and divide by 2 if eventually consistent, divide by 1 if consistent read

On-demand autoscaling for unpredictable traffic patterns - paying for the amount of request
Provisioned autoscaling will have throttling in case there is high demand

## Secondary Indices
    - Local SI
        Same partition but different sort keys (max 5)
    - Global SI
        Different partition and sort key (max 20)
    
Projected Attributes: Keys only, Include or All are the storage possbilities of the secondary index, which basically mirror the data for faster retrieval 

## Global tables 

Enables replication across regions. Needs streams, needs to be setup when the table is empty (!). 

## Item expiration

You can have an Expiration / TTL on items so that they are deleted at a certain time. 

## Backups

Backup of DynamoDB only cost the storage space and when restoring don't restore the access rights, read and write policies are not restored, but indexes are kept. 

### Point-in-Time Recovery 

Creates snapshots to recover, last 5 minute state is available, not closer than that. Aimed for disaster recovery. Max 35 days storage. 

## Monitoring Dynamo DB 

- Metrics for performance 
- Availablity of outages 
