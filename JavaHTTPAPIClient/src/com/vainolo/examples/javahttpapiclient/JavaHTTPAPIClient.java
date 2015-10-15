package com.vainolo.examples.javahttpapiclient;

import com.mashape.unirest.http.HttpResponse;
import com.mashape.unirest.http.JsonNode;
import com.mashape.unirest.http.Unirest;

public class JavaHTTPAPIClient {
	public void getQuestionsUsingUnirest() throws Exception {
		HttpResponse<JsonNode> response = Unirest.get("https://api.stackexchange.com/2.2/questions").
				header("accept",  "application/json").
				queryString("order","desc").
				queryString("sort", "creation").
				queryString("filter", "default").
				queryString("site", "stackoverflow").
				asJson();
		System.out.println(response.getBody().getObject().toString(2));
	}
	
	public static void main(String args[]) throws Exception {
		JavaHTTPAPIClient client = new JavaHTTPAPIClient();
		client.getQuestionsUsingUnirest();
	}
}
