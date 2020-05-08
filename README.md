# trackrequests
Simple code to keep counters of requests in memory

## What is this about
Suppose a server that handles tenths of thousands of requests per minute.
How to display a counter with the requests in the last minute?

## Simplest: count requests per minute and reset on minute change
This would be the simplest: a counter that keeps track of requests in the current minute. When minute changes, the counter is reset to zero.
It is very simple and efficient but it is not a good approach to understand the server load because it is reset every minute.

## Simple but memory inefficient: keep a list of requests
To keep the counter of requests in the last minute (or hour, or whatever) they key is to have some sort of sliding window.

![Sample 00](https://github.com/psantosl/trackrequests/blob/master/doc/00-track.png)

A simple List<Request> will do. It is just a matter of removing requests older than the window we want to track.
At its bare minimum request could be something like this:
```c-sharp
class Request
{
    internal DateTime When;
    internal int ProcessTime;
}
```
In case we not only want to track the request count but also the process time.
DateTime is 8 bytes, int is 4 bytes = 12 bytes per entry. Considering 50k requests/min, we just need 0.5 MB to keep track of a minute. It would be 34 MB for 1 full hour at this rate.
What is the problem with this approach? That the memory required to keep the request count is directly proportional to the number of requests.


## A memory efficient method to track requests
There might be many approaches to do this, but I implemented the following idea, that is not tied to the number of requests processed and only to the number of periods you want to keep track of. It can be 60 minutes, 60 seconds, etc.
Suppose we just need to keep a sliding window of requests in the last 5 seconds:

![Sample 01](https://github.com/psantosl/trackrequests/blob/master/doc/01-track.png)

In my case, I needed to keep counters for about 250 different request types, so the code simply uses a bi-dimensional array as follows:


![Sample 02](https://github.com/psantosl/trackrequests/blob/master/doc/02-track.png)
