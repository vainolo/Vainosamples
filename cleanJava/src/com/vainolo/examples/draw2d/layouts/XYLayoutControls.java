/*******************************************************************************
 * Copyright (c) 2012 Arieh 'Vainolo' Bibliowicz
 * You can use this code for educational purposes. For any other uses
 * please contact me: vainolo@gmail.com
 *******************************************************************************/
package com.vainolo.examples.draw2d.layouts;

import org.eclipse.swt.SWT;
import org.eclipse.swt.events.SelectionListener;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Combo;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Label;
import org.eclipse.swt.widgets.Text;

public class XYLayoutControls extends Composite {
  private Text x;
  private Text y;
  private Button addFigureBtn;

  /**
   * Create the composite.
   * 
   * @param parent
   * @param style
   */
  public XYLayoutControls(Composite parent, int style) {
    super(parent, style);
    setLayout(new GridLayout(4, false));

    Label lblFigurekind = new Label(this, SWT.NONE);
    lblFigurekind.setLayoutData(new GridData(SWT.RIGHT, SWT.FILL, false, false, 1, 1));
    lblFigurekind.setText("Figure Kind");

    Combo combo = new Combo(this, SWT.NONE);
    combo.setItems(new String[] { "Square", "Triangle", "Rectangle" });
    combo.setLayoutData(new GridData(SWT.FILL, SWT.CENTER, true, false, 3, 1));

    Label lblX = new Label(this, SWT.NONE);
    lblX.setLayoutData(new GridData(SWT.RIGHT, SWT.CENTER, false, false, 1, 1));
    lblX.setText("X:");

    x = new Text(this, SWT.BORDER);
    x.setLayoutData(new GridData(SWT.FILL, SWT.CENTER, true, false, 1, 1));

    Label lblNewLabel = new Label(this, SWT.NONE);
    lblNewLabel.setLayoutData(new GridData(SWT.RIGHT, SWT.CENTER, false, false, 1, 1));
    lblNewLabel.setText("Y:");

    y = new Text(this, SWT.BORDER);
    y.setLayoutData(new GridData(SWT.FILL, SWT.CENTER, true, false, 1, 1));
    new Label(this, SWT.NONE);
    new Label(this, SWT.NONE);
    new Label(this, SWT.NONE);

    addFigureBtn = new Button(this, SWT.NONE);
    addFigureBtn.setLayoutData(new GridData(SWT.RIGHT, SWT.CENTER, false, false, 1, 1));
    addFigureBtn.setText("Add Figure");

  }

  @Override
  protected void checkSubclass() {
    // Disable the check that prevents subclassing of SWT components
  }

  public Text getX() {
    return x;
  }

  public Text getY() {
    return y;
  }

  public void addAddFigureBtnListener(SelectionListener listener) {

    addFigureBtn.addSelectionListener(listener);
  }
}
