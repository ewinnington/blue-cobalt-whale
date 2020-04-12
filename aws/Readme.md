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

