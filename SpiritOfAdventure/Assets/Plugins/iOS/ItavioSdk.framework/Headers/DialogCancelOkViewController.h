//
//  pop.h
//  PopupTest
//
//  Created by Joselyn O'Connor on 2015-03-26.
//  Copyright (c) 2015 Joselyn O'Connor. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <QuartzCore/QuartzCore.h>
#import "ITGlobals.h"

@interface DialogCancelOkViewController : UIViewController

@property (weak, nonatomic) IBOutlet UIView *popView;
@property (weak, nonatomic) IBOutlet UILabel *message;


@property (nonatomic, copy) ObjectCallbackBlock OKCallback;

- (void)showInView:(UIView *)aView withMessage:(NSString*)message animated:(BOOL)animated onOk:(ObjectCallbackBlock) cb;

@end