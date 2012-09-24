/*******************************************************************************
 * Copyright (c) 2012 Arieh 'Vainolo' Bibliowicz
 * You can use this code for educational purposes. For any other uses
 * please contact me: vainolo@gmail.com
 *******************************************************************************/
package com.vainolo.examples.file;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.Enumeration;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;

/**
 * Extract a zip file to a folder. No error checking or argument validation is done.
 * 
 * @author vainolo
 */
public class ZipExtractor {
	public static void openZipFile(String zipFilename, String destinationDirname) throws IOException {
		byte[] buffer = new byte[1024];
		int bytesRead = 0;
		File zipFile = new File(zipFilename);
		File destinationDir = new File(destinationDirname);

		ZipFile zip = new ZipFile(zipFile);
		Enumeration<? extends ZipEntry> zipEntries = zip.entries();
		while (zipEntries.hasMoreElements()) {
			ZipEntry entry = zipEntries.nextElement();
			if (entry.isDirectory()) {
				File newDir = new File(destinationDir, entry.getName());
				newDir.mkdirs();
			} else {
				BufferedInputStream inputStream = new BufferedInputStream(zip.getInputStream(entry));
				File outputFile = new File(destinationDir, entry.getName());
				BufferedOutputStream outputStream = new BufferedOutputStream(new FileOutputStream(outputFile));
				while ((bytesRead = inputStream.read(buffer)) != -1) {
					outputStream.write(buffer, 0, bytesRead);
				}
				inputStream.close();
				outputStream.close();
			}
		}
	}
}