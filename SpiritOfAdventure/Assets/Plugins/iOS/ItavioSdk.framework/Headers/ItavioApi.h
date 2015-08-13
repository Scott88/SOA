//
//  ItavioSdk.h
//  KgSdkPlugin
//
//  Created by Matthew Pichette on 2015-02-22.
//  Copyright (c) 2015 KinderGuardian Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>
#import "ITGlobals.h"
#import "ItavioDialog.h"

#ifndef ItavioSdk_ItavioApi_h
#define ItavioSdk_ItavioApi_h

@interface ITSdk : NSObject
+ (instancetype) sharedInstance;

- (void) initialize: (ITCredentials *) credentials;
- (void) initialize: (ITCredentials *) credentials onEnvironment: (int) environment;

- (instancetype) initWithCredentials: (ITCredentials *) credentials;
- (instancetype) initWithCredentials: (ITCredentials *) credentials onEnvironment: (int) environment;

- (void) startDebit:(double)amount withCurrencyCode:(NSString *)currencyCode onSuccess:(ITBoolBlock)success onFailure:(ITFailureBlock)failure;

- (void) cancelDebit: (ITBoolBlock) success onFailure: (ITFailureBlock) failure;

- (void) completeDebit: (ITBoolBlock) success onFailure: (ITFailureBlock) failure;

- (BOOL) checkForParent;
- (BOOL) checkForParent: (BOOL)showGetAppDialog;

- (void) getBalance: (ITDoubleBlock) success;

- (BOOL) isEnabled;

- (BOOL) hasLink;

- (void) linkExternal: (BOOL) showGetAppDialog onSuccess: (ITLinkAccountBlock) success onFailure: (ITFailureBlock) failure;
- (void) linkExternal: (BOOL) showGetAppDialog shouldRelink: (BOOL) relink  onSuccess: (ITLinkAccountBlock) success onFailure: (ITFailureBlock) failure;

- (BOOL) handleLink: (NSURL *) url;

- (void) login : (NSString *) email withPassword: (NSString *) password onSuccess: (ITBoolBlock) success onFailure: (ITFailureBlock) failure;

@end

@interface SettingsStore : NSObject

+ (instancetype) sharedInstance;
- (id) init;
- (void) save;
- (void) load;

@property (nonatomic, strong) NSUUID *parentId;
@property (nonatomic, strong) NSUUID *childId;
@property (nonatomic, strong) NSUUID *limitId;
@property (nonatomic, strong) NSNumber *enabled;
@property (nonatomic, strong) NSNumber *txnId;
@property (nonatomic, strong) NSString *email;

@end

#endif
