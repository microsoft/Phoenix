// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

//*********************************************************************
/// 
/// <summary>
/// Interface for implementing Plan Services. Multiple variants of plan
/// services can be created based on this.
/// </summary>
/// 
//*********************************************************************

module PlanServices {
    export interface IPlanServiceInterface {
        initializeServiceOffer(serviceOffer);
        executeCommand();
        initializePage(global);
    }
} 