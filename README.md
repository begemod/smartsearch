#### This is the implementattion of a technical assessment I had to done as a part of hiring process.  

# Briefing  

***Background***  
*Search is a critical component of our web application. In this assessment you will build a simple search functionality using AWS ElasticSearch.You may setup an account with  andutilize “free-tier” so it does not cost you anything.*  

*Your Search REST API needs to accept an input string and output the most relevant records that match.*

***Input***  
*Phrase: (required, string)*  
*Market: (optional, string)*  
*Limit: (int, default: 25, max how many results to be returned)*  

*Feel free to include  other optional input fields you deem necessary.*  

*The assessment folder contains two files ‘properties.json’ and ‘mgmt.json’.*  
* *properties.json represents apartment buildings.*  
* *mgmt.json represents the management companies that own the apartment buildings.*   

***Output***  
*Your REST API should return the most relevant search results. ElasticSearch has many different types of fulltext search options.*   
*As such, you need to select the correct one.  The user will enter a phrase that contains stop words like “and”, “or”, “into” “the”.*  
*A substring match is a dead-end because stop-words will be matched and hence results will not be returned correctly.*  
*This is why fulltext is so much more power. Your full text search select needs to account for the highest accuracy. For example: when the user types “stones and rocks” the search should return results that match “stone rocks apartments”.  Notice:  The user mistyped stones (extra s) , added stop word “and”.*
