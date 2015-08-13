//
//  Globals.h
//  KgSdkPlugin
//
//  Created by Matthew Pichette on 2015-02-22.
//  Copyright (c) 2015 KinderGuardian Inc. All rights reserved.
//


#import <Foundation/Foundation.h>

#ifndef ItavioSdk_Globals_h
#define ItavioSdk_Globals_h

#define ITAVIO_ERROR_DOMAIN @"ITAVIO"

// Error domains that this framework will use in error callbacks
extern NSString * const ITErrorDomain;

typedef void (^ITLinkAccountBlock)(BOOL installed, BOOL linked);
typedef void (^ITBoolBlock)(BOOL proceed);
//typedef void (^ITPurchaseBlock)(BOOL canPurchase, BOOL sufficientFunds);
typedef void (^ITIdBlock)(id);
typedef void (^ObjectCallbackBlock)(NSError **, id);
typedef void (^ITDoubleBlock)(double value);
typedef BOOL (^PassFailCallbackBlock)(NSError **);
typedef int (^IntStatusCallbackBlock)(NSError **);
typedef void (^YesNoCallbackBlock)(NSError **, NSNumber *);
typedef void (^BoolCallbackBlock)(NSError **, BOOL);
typedef void(^ITFailureBlock)(NSError ** error);

@interface ITCredentials : NSObject
@property (nonatomic, copy) NSString *_secretKeyId;
@property (nonatomic, copy) NSString *_secretKey;
- (id) init: (NSString *) secretKeyId securedBy: (NSString *) secretKey;
@end

@interface ITParentCredentials : NSObject
@property (nonatomic, copy) NSString *email;
@property (nonatomic, copy) NSString *password;
@end

typedef NS_ENUM(NSInteger, ErrorCodes) {
    InsufficientFundsError,
    ConflictingTransactionError,
    UnauthorizedError,
    InternetNotReachableError,
    UnhandledError
};

static inline NSString* NSStringFromBOOL(BOOL aBool) {
    return aBool? @"true" : @"false"; }

#endif
