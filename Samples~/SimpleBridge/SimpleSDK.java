package com.example.sdk;

import android.content.Context;

/**
 * Simple example SDK for demonstrating bridge generation
 * This is a sample Java class that can be used to test the bridge generator
 */
public class SimpleSDK {
    private static SimpleSDK instance;
    private Context context;
    private boolean initialized = false;
    
    /**
     * Initialize the SDK
     */
    public static void init(Context context) {
        if (instance == null) {
            instance = new SimpleSDK();
            instance.context = context;
            instance.initialized = true;
        }
    }
    
    /**
     * Get SDK instance
     */
    public static SimpleSDK getInstance() {
        return instance;
    }
    
    /**
     * Check if SDK is initialized
     */
    public static boolean isInitialized() {
        return instance != null && instance.initialized;
    }
    
    /**
     * Get a welcome message
     */
    public static String getMessage() {
        return "Hello from SimpleSDK!";
    }
    
    /**
     * Calculate sum of two numbers
     */
    public static int calculate(int a, int b) {
        return a + b;
    }
    
    /**
     * Get SDK version
     */
    public static String getVersion() {
        return "1.0.0";
    }
    
    /**
     * Check feature support
     */
    public static boolean supportsFeature(String featureName) {
        return featureName.equals("basic");
    }
    
    /**
     * Process array of values
     */
    public static int[] processValues(int[] values) {
        int[] result = new int[values.length];
        for (int i = 0; i < values.length; i++) {
            result[i] = values[i] * 2;
        }
        return result;
    }
}
