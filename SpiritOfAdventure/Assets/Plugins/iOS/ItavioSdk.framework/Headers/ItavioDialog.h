//
//  DialogViewController.h
//  ItavioSdk
//
//  Created by Joselyn O'Connor on 2015-04-02.
//  Copyright (c) 2015 Itavio. All rights reserved.
//

#ifndef ItavioSdk_DialogViewController_h
#define ItavioSdk_DialogViewController_h

#import <UIKit/UIKit.h>
#import "DialogOkViewController.h"
#import "DialogGetAppViewController.h"
#import "DialogUninstalledAppViewController.h"
#import "DialogCancelOkViewController.h"
#import <CoreText/CoreText.h>
#import <Foundation/Foundation.h>
#import "ITGlobals.h"

@interface ItavioDialog : NSObject
typedef NS_ENUM(NSUInteger, DialogMessage) {
    errorGeneric,
    errorFailedToInitialize,
    errorInvalidCredentials,
    errorTransactionStart,
    errorTransactionComplete,
    errorHasNotInit,
    errorInsufficientFunds,
    errorConflictingTransaction,
    errorLimitNotFound,
    errorCancelTransaction,
    
    messageConfirmLink
};

@property (strong, nonatomic) IBOutlet DialogOkViewController* dOkViewController;
@property (strong, nonatomic) IBOutlet DialogGetAppViewController* dGAViewController;
@property (strong, nonatomic) IBOutlet DialogUninstalledAppViewController* dUAViewController;
@property (strong, nonatomic) IBOutlet DialogCancelOkViewController* dCOkViewController;

+(instancetype)sharedInstance;

-(void)initDialog;
-(void)showOkDialog:(DialogMessage)message;
-(void)showGetAppDialog;
-(void)showUninstalledAppDialog:(NSString *)email;
-(void)showCancelOkDialog:(DialogMessage)message onOk:(ObjectCallbackBlock)cb;

-(UIViewController *)topViewController;
-(UIViewController *)topViewController:(UIViewController *)rootViewController;
@end


#endif
