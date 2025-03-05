//  ReplayKitHelper.m
//  Unity-iPhone
//
//  Created by Dona Albadrani on 22/01/2025.
//

#ifdef __cplusplus
extern "C" {
#endif

void StartRecording();
void StopRecording();
void SaveReplayToCameraRoll();

#ifdef __cplusplus
}
#endif

#import <Foundation/Foundation.h>
#import <ReplayKit/ReplayKit.h>
#import <UIKit/UIKit.h>
#import <Photos/Photos.h>

// Delegate implementation for RPPreviewViewController
@interface ReplayKitHelper : NSObject <RPPreviewViewControllerDelegate>
@end

@implementation ReplayKitHelper

- (void)previewControllerDidFinish:(RPPreviewViewController *)previewController {
    NSLog(@"previewControllerDidFinish triggered. Dismissing the preview controller.");
    [previewController dismissViewControllerAnimated:YES completion:^{
        NSLog(@"Preview controller dismissed.");
    }];
}


@end

void StartRecording() {
    if (@available(iOS 11.0, *)) {
        if (RPScreenRecorder.sharedRecorder.isAvailable) {
            [RPScreenRecorder.sharedRecorder startRecordingWithHandler:^(NSError * _Nullable error) {
                if (error) {
                    NSLog(@"Error starting recording: %@", error.localizedDescription);
                } else {
                    NSLog(@"Recording started.");
                }
            }];
        } else {
            NSLog(@"ReplayKit is not available on this device.");
        }
    } else {
        NSLog(@"ReplayKit requires iOS 11.0 or later.");
    }
}


void StopRecording() {
    if (@available(iOS 11.0, *)) {
        if (RPScreenRecorder.sharedRecorder.isRecording) {
            [RPScreenRecorder.sharedRecorder stopRecordingWithHandler:^(RPPreviewViewController * _Nullable previewViewController, NSError * _Nullable error) {
                if (error) {
                    NSLog(@"Error stopping recording: %@", error.localizedDescription);
                    return;
                }

                if (previewViewController) {
                    // Retain the delegate instance globally
                    static ReplayKitHelper *helper = nil;
                    helper = [[ReplayKitHelper alloc] init];

                    // Set the delegate
                    previewViewController.previewControllerDelegate = helper;

                    // Get the root view controller
                    UIViewController *rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;

                    // Handle iPad-specific popover presentation
//                    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
//                        previewViewController.modalPresentationStyle = UIModalPresentationPopover;
//                        UIPopoverPresentationController *popover = previewViewController.popoverPresentationController;
//                        popover.sourceView = rootViewController.view;
//                        popover.sourceRect = CGRectMake(CGRectGetMidX(rootViewController.view.bounds),
//                                                        CGRectGetMidY(rootViewController.view.bounds),
//                                                        0, 0);
//                        popover.permittedArrowDirections = UIPopoverArrowDirectionAny;
//                    }
                    
                    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
                        previewViewController.modalPresentationStyle = UIModalPresentationPopover;
                        UIPopoverPresentationController *popover = previewViewController.popoverPresentationController;

                        popover.sourceView = rootViewController.view; // Use the root view as the source
                        popover.sourceRect = CGRectMake(rootViewController.view.bounds.size.width / 2.0,
                                                        rootViewController.view.bounds.size.height / 2.0,
                                                        -1000, 0); // Center the popover
                        popover.permittedArrowDirections = UIPopoverArrowDirectionUnknown; // Remove arrow
                    }


                    // Present the preview controller
                    [rootViewController presentViewController:previewViewController animated:YES completion:^{
                        NSLog(@"Preview controller presented.");
                    }];
                } else {
                    NSLog(@"No preview controller available.");
                }
            }];
        } else {
            NSLog(@"ReplayKit is not currently recording.");
        }
    } else {
        NSLog(@"ReplayKit requires iOS 11.0 or later.");
    }
}




void SaveReplayToCameraRoll() {
    if (@available(iOS 11.0, *)) {
        if (RPScreenRecorder.sharedRecorder.isAvailable && !RPScreenRecorder.sharedRecorder.isRecording) {
            [RPScreenRecorder.sharedRecorder stopRecordingWithHandler:^(RPPreviewViewController * _Nullable previewViewController, NSError * _Nullable error) {
                if (error) {
                    NSLog(@"Error stopping recording: %@", error.localizedDescription);
                    return;
                }

                if (previewViewController) {
                    // Get the recording file URL
                    NSURL *recordingURL = [previewViewController valueForKey:@"movieURL"];
                    if (recordingURL) {
                        // Save the video to Camera Roll
                        [[PHPhotoLibrary sharedPhotoLibrary] performChanges:^{
                            [PHAssetChangeRequest creationRequestForAssetFromVideoAtFileURL:recordingURL];
                        } completionHandler:^(BOOL success, NSError * _Nullable saveError) {
                            if (success) {
                                NSLog(@"Recording saved to Camera Roll.");
                            } else {
                                NSLog(@"Error saving to Camera Roll: %@", saveError.localizedDescription);
                            }
                        }];
                    } else {
                        NSLog(@"No movie URL available.");
                    }
                } else {
                    NSLog(@"No preview controller available.");
                }
            }];
        } else {
            NSLog(@"ReplayKit is not available or recording.");
        }
    } else {
        NSLog(@"ReplayKit requires iOS 11.0 or later.");
    }
}
