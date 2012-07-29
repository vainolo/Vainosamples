package com.vainolo.examples.draw2d;

import org.eclipse.draw2d.Figure;
import org.eclipse.draw2d.IFigure;
import org.eclipse.draw2d.LightweightSystem;
import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.SashForm;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.widgets.Canvas;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Shell;

public abstract class AbstractDraw2DExample {

  private Figure contents;
  private Shell shell;
  private Display display;
  private Canvas canvas;
  private Composite right;

  public AbstractDraw2DExample(String name) {
    display = new Display();
    shell = new Shell(display, SWT.SHELL_TRIM);
    shell.setText(name);
    shell.setLayout(new FillLayout());

    SashForm sashForm = new SashForm(shell, SWT.SMOOTH | SWT.BORDER);

    Composite left = new Composite(sashForm, SWT.NONE);
    left.setLayout(new FillLayout());

    canvas = new Canvas(left, SWT.NONE);
    canvas.setBackground(display.getSystemColor(SWT.COLOR_WHITE));

    LightweightSystem lws = new LightweightSystem(canvas);
    contents = new Figure();
    lws.setContents(contents);

    right = new Composite(sashForm, SWT.NONE);
  }

  public void run() {
    shell.open();
    shell.layout();
    while(!shell.isDisposed()) {
      while(!display.readAndDispatch()) {
        display.sleep();
      }
    }
    display.dispose();
  }

  protected IFigure getDraw2DContents() {
    return contents;
  }

  protected Composite getRightComposite() {
    return right;
  }

  protected Shell getShell() {
    return shell;
  }
}