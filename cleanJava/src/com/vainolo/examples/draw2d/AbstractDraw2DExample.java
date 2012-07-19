package com.vainolo.examples.draw2d;

import org.eclipse.draw2d.Figure;
import org.eclipse.draw2d.IFigure;
import org.eclipse.draw2d.LightweightSystem;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Shell;

public abstract class AbstractDraw2DExample {

  private Figure contents;

  public AbstractDraw2DExample(String name) {
    Display d = new Display();
    Shell shell = new Shell(d);
    shell.setText(name);
    LightweightSystem lws = new LightweightSystem(shell);
    contents = new Figure();
    lws.setContents(contents);
    shell.open();
    while(!shell.isDisposed()) {
      while(!d.readAndDispatch()) {
        d.sleep();
      }
    }
  }

  protected IFigure getFigure() {
    return contents;
  }
}