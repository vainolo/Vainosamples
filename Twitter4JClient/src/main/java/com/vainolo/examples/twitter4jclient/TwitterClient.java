package com.vainolo.examples.twitter4jclient;

import twitter4j.Query;
import twitter4j.QueryResult;
import twitter4j.Status;
import twitter4j.Twitter;
import twitter4j.TwitterException;
import twitter4j.TwitterFactory;
import twitter4j.conf.ConfigurationBuilder;

public class TwitterClient {

  public static void main(String args[]) throws TwitterException {
    ConfigurationBuilder cb = new ConfigurationBuilder();
    cb.setDebugEnabled(true)
      .setOAuthConsumerKey("YOUR KEY")
      .setOAuthConsumerSecret("YOUR SECRET")
      .setOAuthAccessToken("YOUR TOKEN")
      .setOAuthAccessTokenSecret("YOUR TOKEN SECRET");
    TwitterFactory tf = new TwitterFactory(cb.build());
    Twitter twitter = tf.getInstance();    
    Query query = new Query("from:elvis");
    QueryResult result = twitter.search(query);
    
    for (Status status : result.getTweets()) {
        System.out.println("@" + status.getUser().getScreenName() + ":" + status.getText());
    }    
    
  }
}
