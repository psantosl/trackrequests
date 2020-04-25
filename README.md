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


