/*******************************************************************************
 * Copyright (c) 2012 Arieh 'Vainolo' Bibliowicz
 * You can use this code for educational purposes. For any other uses
 * please contact me: vainolo@gmail.com
 *******************************************************************************/
package com.vainolo.examples.net;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.net.URL;
import java.net.URLConnection;

/**
 * Copy a file from a network address to a local file. No error checking or argument validation is done.
 * 
 * @author vainolo
 * 
 */
public class NetworkFileCopier {
	public static File copyFileFromWeb(String address, String filePath) throws Exception {
		byte[] buffer = new byte[1024];
		int bytesRead;

		URL url = new URL(address);
		BufferedInputStream inputStream = null;
		BufferedOutputStream outputStream = null;
		URLConnection connection = url.openConnection();
		// If you need to use a proxy for your connection, the URL class has another openConnection method.
		// For example, to connect to my local SOCKS proxy I can use:
		// url.openConnection(new Proxy(Proxy.Type.SOCKS, newInetSocketAddress("localhost", 5555)));
		inputStream = new BufferedInputStream(connection.getInputStream());
		File f = new File(filePath);
		outputStream = new BufferedOutputStream(new FileOutputStream(f));
		while ((bytesRead = inputStream.read(buffer)) != -1) {
			outputStream.write(buffer, 0, bytesRead);
		}
		inputStream.close();
		outputStream.close();
		return f;
	}
}
