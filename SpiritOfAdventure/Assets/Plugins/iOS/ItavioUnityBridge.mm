//
//  ItavioUnityBridge.m
//  ItavioSdkTester
//
//  Created by Joselyn O'Connor on 2015-04-29.
//  Copyright (c) 2015 Itavio. All rights reserved.
//

#import <ItavioSdk/ItavioSdk.h>

#define UNITY_CLASS "ItavioManager"

#define UNITY_ON_GET_BALANCE "onGetBalanceSuccess"
#define UNITY_ERROR_ON_GET_BALANCE "onGetBalanceError"
#define UNITY_ON_START_DEBIT "onStartDebitSuccess"
#define UNITY_ERROR_ON_START_DEBIT "onStartDebitError"
#define UNITY_ON_CANCEL_DEBIT "onCancelDebitSuccess"
#define UNITY_ERROR_ON_CANCEL_DEBIT "onCancelDebitError"
#define UNITY_ON_COMPLETE_DEBIT "onCompleteDebitSuccess"
#define UNITY_ERROR_ON_COMPLETE_DEBIT "onCompleteDebitError"
#define UNITY_ON_CHECK_FOR_PARENT "onCheckForParentSuccess"
#define UNITY_ERROR_ON_CHECK_FOR_PARENT "onCheckForParentError"

#define UNITY_ON_LINK "onLinkSuccess"
#define UNITY_ERROR_ON_LINK "onLinkError"


char* CStringFromNSString(NSString* oldstring);
char* NSDictToJsonCString(NSDictionary* dict);
static NSString* NSStringFromCString(const char* string);

extern "C" {
    int itavioInitialize(const char* secretKeyId, const char* secretKey, int environment)
    {
        [[ITSdk sharedInstance] initialize:[[ITCredentials alloc] init: NSStringFromCString(secretKeyId) securedBy: NSStringFromCString(secretKey)] onEnvironment: environment ];
        return 0;
    }
    
    void itavioGetBalance()
    {
        [[ITSdk sharedInstance] getBalance:^(double value) {
            dispatch_async(dispatch_get_main_queue(), ^{
                NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys: [NSString stringWithFormat:@"%lf", value], @"balance", nil];
                UnitySendMessage(UNITY_CLASS, UNITY_ON_GET_BALANCE, NSDictToJsonCString(dict));
            });
        }];
    }
    
    void itavioStartDebit(double amount, const char* currencyCode)
    {
        [[ITSdk sharedInstance] startDebit:amount withCurrencyCode:NSStringFromCString(currencyCode) onSuccess:^(BOOL proceed) {
            if (proceed) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    // Purchase has start
                    NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys: @YES, @"enabled", @YES, @"hasFunds", nil];
                    UnitySendMessage(UNITY_CLASS, UNITY_ON_START_DEBIT, NSDictToJsonCString(dict));
                });
            }
            else {
                dispatch_async(dispatch_get_main_queue(), ^{
                    // Purchase failed
                    UnitySendMessage(UNITY_CLASS, UNITY_ERROR_ON_START_DEBIT, "");
                });
            }
        } onFailure:^(NSError *__autoreleasing *error) {
            dispatch_async(dispatch_get_main_queue(), ^{
                // Purchase failed
                UnitySendMessage(UNITY_CLASS, UNITY_ERROR_ON_START_DEBIT, "");
            });
        }];
    }
    
    void itavioCancelDebit()
    {
        [[ITSdk sharedInstance] cancelDebit:^(BOOL proceed) {
            if (proceed) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    // The debit has been canceled
                    UnitySendMessage(UNITY_CLASS, UNITY_ON_CANCEL_DEBIT, "");
                });
            }
            else {
                dispatch_async(dispatch_get_main_queue(), ^{
                    // An error has occurred
                    UnitySendMessage(UNITY_CLASS, UNITY_ERROR_ON_CANCEL_DEBIT, "");
                });
                
            }
        } onFailure:^(NSError *__autoreleasing *error) {
            dispatch_async(dispatch_get_main_queue(), ^{
                // An error has occurred
                UnitySendMessage(UNITY_CLASS, UNITY_ERROR_ON_CANCEL_DEBIT, "");
            });
        }];
    }
    
    void itavioCompleteDebit()
    {
        [[ITSdk sharedInstance] completeDebit:^(BOOL proceed) {
            if (proceed) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    // The purchase has been processed
                    UnitySendMessage(UNITY_CLASS, UNITY_ON_COMPLETE_DEBIT, "");
                });
            }
            else {
                dispatch_async(dispatch_get_main_queue(), ^{
                    // An error has occurred
                    UnitySendMessage(UNITY_CLASS, UNITY_ERROR_ON_COMPLETE_DEBIT, "");
                });
                
            }
        } onFailure:^(NSError *__autoreleasing *error) {
            dispatch_async(dispatch_get_main_queue(), ^{
                // An error has occurred
                UnitySendMessage(UNITY_CLASS, UNITY_ERROR_ON_COMPLETE_DEBIT, "");
            });
        }];
    }
    
    void itavioCheckForParent(bool showGetAppDialog, bool ignoreSuppression)
    {
        if ([[ITSdk sharedInstance] checkForParent:showGetAppDialog shouldIgnoreSuppression:ignoreSuppression]) {
            // Itavio Parent App is installed
            NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys: @YES, @"hasParentApp", nil];
            UnitySendMessage(UNITY_CLASS, UNITY_ON_CHECK_FOR_PARENT, NSDictToJsonCString(dict));
        }
        else {
            // Itavio Parent App is installed
            NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys: @NO, @"hasParentApp", nil];
            UnitySendMessage(UNITY_CLASS, UNITY_ON_CHECK_FOR_PARENT, NSDictToJsonCString(dict));
        }
    }
    
    bool itavioHasLink()
    {
        return [[ITSdk sharedInstance] hasLink];
    }
    
    void itavioLinkWithParentApp(bool showGetAppDialog, bool relink)
    {
        [[ITSdk sharedInstance] linkExternal:showGetAppDialog shouldRelink:relink onSuccess:^(BOOL installed, BOOL linked) {
            if (linked) {
                // Linking succeeded
                UnitySendMessage(UNITY_CLASS, UNITY_ON_LINK, "");
            }
            else {
                if (installed) {
                    // Linking failed (parent app is present)
                    NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys: @YES, @"hasParentApp", nil];
                    UnitySendMessage(UNITY_CLASS, UNITY_ERROR_ON_LINK, NSDictToJsonCString(dict));
                }
                else {
                    // Linking failed (parent app is missing)
                    NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys: @NO, @"hasParentApp", nil];
                    UnitySendMessage(UNITY_CLASS, UNITY_ERROR_ON_LINK, NSDictToJsonCString(dict));
                }
            }
        } onFailure:^(NSError *__autoreleasing *error) {
            // Linking failed
            UnitySendMessage(UNITY_CLASS, UNITY_ERROR_ON_LINK, "");
        }];
    }
}

char* CStringFromNSString(NSString* oldstring)
{
    const char* string = [oldstring UTF8String];
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

char* NSDictToJsonCString(NSDictionary* dict)
{
    NSError *e;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject: dict options: 0 error: &e];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    
    return CStringFromNSString(jsonString);
}

// This takes a char* you get from Unity and converts it to an NSString* to use in your objective c code. You can mix c++ and objective c all in the same file.
static NSString* NSStringFromCString(const char* string)
{
    if (string != NULL)
        return [NSString stringWithUTF8String:string];
    else
        return [NSString stringWithUTF8String:""];
}