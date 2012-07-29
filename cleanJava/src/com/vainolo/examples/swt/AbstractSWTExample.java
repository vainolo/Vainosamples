/*******************************************************************************
 * Copyright (c) 2012 Arieh 'Vainolo' Bibliowicz
 * You can use this code for educational purposes. For any other uses
 * please contact me: vainolo@gmail.com
 *******************************************************************************/
package com.vainolo.examples.swt;

import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Shell;

public abstract class AbstractSWTExample {
  private Shell shell;
  private Display display;

  public AbstractSWTExample(String name) {
    display = new Display();
    shell = new Shell(display);
    shell.setText(name);
  }

  public void run() {
    shell.open();
    while(!shell.isDisposed()) {
      while(!display.readAndDispatch()) {
        display.sleep();
      }
    }
    display.dispose();
  }

  public Shell getShell() {
    return shell;
  }
}
