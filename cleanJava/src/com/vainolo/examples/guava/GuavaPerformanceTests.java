/*******************************************************************************
 * Copyright (c) 2012 Arieh 'Vainolo' Bibliowicz
 * You can use this code for educational purposes. For any other uses
 * please contact me: vainolo@gmail.com
 *******************************************************************************/
package com.vainolo.examples.guava;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Random;

import com.google.common.base.Predicate;
import com.google.common.collect.Collections2;

public class GuavaPerformanceTests {

  private Random rand = new Random();
  private int numberOfIterations;
  private int elementRange;
  private int numberOfElements;

  static int counter = 0;

  public GuavaPerformanceTests(int numberOfIterations, int numberOfElements, int elementRange) {
    this.numberOfIterations = numberOfIterations;
    this.numberOfElements = numberOfElements;
    this.elementRange = elementRange;
  }

  private Collection<Integer> initializeSource() {
    List<Integer> source = new ArrayList<Integer>();
    for(int i = 0; i < numberOfElements; i++) {
      source.add(rand.nextInt(elementRange));
    }
    return source;
  }

  private Predicate<Integer> initializePredicate() {
    final int range = rand.nextInt(elementRange);
    Predicate<Integer> predicate = new Predicate<Integer>() {
      @Override
      public boolean apply(Integer input) {
        return input < range;
      }
    };
    return predicate;
  }

  private Collection<Integer> copyFilter(Collection<Integer> source, Predicate<Integer> p) {
    ArrayList<Integer> retVal = new ArrayList<Integer>();
    for(Integer integer : source) {
      if(p.apply(integer))
        retVal.add(integer);
    }
    return retVal;
  }

  private Collection<Integer> guavaFilter(Collection<Integer> source, Predicate<Integer> p) {
    return Collections2.filter(source, p);
  }

  private TestResult run() {
    TestResult result = new TestResult();
    long start, end;
    Collection<Integer> source = initializeSource(), target;

    for(int i = 0; i < numberOfIterations; i++) {
      Predicate<Integer> predicate = initializePredicate();

      // Copy tests
      start = System.currentTimeMillis();
      target = copyFilter(source, predicate);
      end = System.currentTimeMillis();
      result.copyFilterTime += (end - start);

      start = System.currentTimeMillis();
      for(int j : target) {
        counter += j;
      }
      end = System.currentTimeMillis();
      result.copyAccessTime += (end - start);

      start = System.currentTimeMillis();
      for(int k = 0; k < 10; k++) {
        for(int j : target) {
          counter += j;
        }
      }
      end = System.currentTimeMillis();
      result.multipleCopyAccessTime += (end - start);

      // Guava tests
      start = System.currentTimeMillis();
      target = guavaFilter(source, predicate);
      end = System.currentTimeMillis();
      result.guavaFilterTime += (end - start);

      start = System.currentTimeMillis();
      for(int j : target) {
        counter += j;
      }
      end = System.currentTimeMillis();
      result.guavaAccessTime += (end - start);

      start = System.currentTimeMillis();
      for(int k = 0; k < 10; k++) {
        for(int j : target) {
          counter += j;
        }
      }
      end = System.currentTimeMillis();
      result.multipleGuavaAccessTime += (end - start);

    }

    return result;
  }

  public static void main(String args[]) {
    int[] numberOfElements = { 100, 1000, 10000 };
    int[] elementRange = { 10, 100, 1000, 10000 };
    System.out
        .println("#Elements,Range,Copy Filter,Copy Access,Guava Filter,Guava Access,Multiple Copy Access, Multiple Guava Access");
    for(int elements : numberOfElements) {
      for(int range : elementRange) {
        GuavaPerformanceTests tests = new GuavaPerformanceTests(1000, elements, range);
        TestResult results = tests.run();
        System.out.println("" + elements + "," + range + "," + results.copyFilterTime + "," + results.guavaFilterTime +
            "," + results.copyAccessTime + "," + results.guavaAccessTime + "," + results.multipleCopyAccessTime + "," +
            results.multipleGuavaAccessTime);
      }
    }

    System.out.println("\n" + counter);

  }

  private class TestResult {
    long copyFilterTime = 0;
    long guavaFilterTime = 0;
    long copyAccessTime = 0;
    long guavaAccessTime = 0;
    long multipleCopyAccessTime = 0;
    long multipleGuavaAccessTime = 0;
  }
}
