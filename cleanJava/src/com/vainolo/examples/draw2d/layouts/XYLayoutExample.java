/*******************************************************************************
 * Copyright (c) 2012 Arieh 'Vainolo' Bibliowicz
 * You can use this code for educational purposes. For any other uses
 * please contact me: vainolo@gmail.com
 *******************************************************************************/
package com.vainolo.examples.draw2d.layouts;

import org.eclipse.draw2d.ColorConstants;
import org.eclipse.draw2d.Figure;
import org.eclipse.draw2d.RectangleFigure;
import org.eclipse.draw2d.XYLayout;
import org.eclipse.draw2d.geometry.Rectangle;
import org.eclipse.swt.SWT;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.events.SelectionListener;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.widgets.Composite;

import com.vainolo.examples.draw2d.AbstractDraw2DExample;

public class XYLayoutExample extends AbstractDraw2DExample {

  private static final String exampleName = "XYLayoutExample";

  public XYLayoutExample() {
    super(exampleName);
    setLayoutManager();
    addFigures();
    initControls();
  }

  private void setLayoutManager() {
    getDraw2DContents().setLayoutManager(new XYLayout());
  }

  private void initControls() {
    Composite right = getRightComposite();
    right.setLayout(new FillLayout());
    XYLayoutControls controls = new XYLayoutControls(right, SWT.NONE);
    controls.addAddFigureBtnListener(new SelectionListener() {

      @Override
      public void widgetSelected(SelectionEvent e) {
        System.out.println("Here");

      }

      @Override
      public void widgetDefaultSelected(SelectionEvent e) {
      }
    });
  }

  private void addFigures() {
    Figure f1 = new RectangleFigure();
    f1.setForegroundColor(ColorConstants.black);
    getDraw2DContents().add(f1, new Rectangle(50, 50, 100, 100));
    getDraw2DContents().revalidate();
  }

  public static void main(String args[]) {
    XYLayoutExample example = new XYLayoutExample();
    example.run();
  }

}
