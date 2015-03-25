/*
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS HEADER.
 *
 * Copyright (c) 2009 Sun Microsystems Inc. All Rights Reserved
 *
 * The contents of this file are subject to the terms
 * of the Common Development and Distribution License
 * (the License). You may not use this file except in
 * compliance with the License.
 *
 * You can obtain a copy of the License at
 * https://opensso.dev.java.net/public/CDDLv1.0.html or
 * opensso/legal/CDDLv1.0.txt
 * See the License for the specific language governing
 * permission and limitations under the License.
 *
 * When distributing Covered Code, include this CDDL
 * Header Notice in each file and include the License file
 * at opensso/legal/CDDLv1.0.txt.
 * If applicable, add the following below the CDDL Header,
 * with the fields enclosed by brackets [] replaced by
 * your own identifying information:
 * "Portions Copyrighted [year] [name of copyright owner]"
 *
 * $Id: HOTP.java,v 1.1 2009/03/24 23:52:12 pluo Exp $
 *
 */
/*
 * Portions Copyrighted 2012-2013 ForgeRock AS
 */

package com.sun.identity.authentication.modules.iccard;

import java.security.Principal;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.ResourceBundle;
import java.util.Set;

import javax.security.auth.Subject;
import javax.security.auth.callback.Callback;
import javax.security.auth.callback.PasswordCallback;

import com.iplanet.sso.SSOException;
import com.sun.identity.authentication.spi.AMLoginModule;
import com.sun.identity.authentication.spi.AuthLoginException;
import com.sun.identity.authentication.util.ISAuthConstants;
import com.sun.identity.idm.AMIdentity;
import com.sun.identity.idm.AMIdentityRepository;
import com.sun.identity.idm.IdRepoException;
import com.sun.identity.idm.IdSearchControl;
import com.sun.identity.idm.IdSearchOpModifier;
import com.sun.identity.idm.IdSearchResults;
import com.sun.identity.idm.IdType;
import com.sun.identity.shared.datastruct.CollectionHelper;
import com.sun.identity.shared.debug.Debug;

public class ICCard extends AMLoginModule {

	// Variables
    protected static final String amAuthICCard = "amAuthICCard";
    protected static final Debug debug = Debug.getInstance(amAuthICCard);
    ResourceBundle bundle = null;

    protected Principal userPrincipal;
    private Map sharedState;

    private String authAttribute = null;
    private String customUri = null;
    private String userName = null;

    // Module specific properties
    private static String AUTHLEVEL = "iplanet-am-auth-iccard-auth-level";
    private static String AUTHATTRIBUTE = "iplanet-am-auth-iccard-auth-attribute-name";
    private static String CUSTOMEURI = "iplanet-am-auth-iccard-client-custom-schema";

    /**
     * Initialize parameters.
     *
     * @param subject
     * @param sharedState
     * @param options
     */
    public void init(Subject subject, Map sharedState, Map options) {

    	String authLevel = CollectionHelper.getMapAttr(options, AUTHLEVEL);
        if (authLevel != null) {
            try {
                setAuthLevel(Integer.parseInt(authLevel));
            } catch (Exception e) {
                debug.error("ICCard.init() : " + "Unable to set auth level " + authLevel, e);
            }
        }

        authAttribute = CollectionHelper.getMapAttr(options, AUTHATTRIBUTE);
        customUri = CollectionHelper.getMapAttr(options, AUTHATTRIBUTE);

        java.util.Locale locale = getLoginLocale();
        bundle = amCache.getResBundle(amAuthICCard, locale);
        if (debug.messageEnabled()) {
            debug.message("DataStore resbundle locale=" + locale);
        }

        this.sharedState = sharedState;

    }

    /**
     * Returns principal of the authenticated user.
     *
     * @return Principal of the authenticated user.
     */
    public Principal getPrincipal() {
        if (userPrincipal != null) {
            return userPrincipal;
        }

        if (userName != null) {
            userPrincipal = new ICCardPrincipal(userName);
            return userPrincipal;
        }

        return null;

    }

    public int process(Callback[] callbacks, int state)
            throws AuthLoginException {

        int result = ISAuthConstants.LOGIN_IGNORE;

        String org = getRequestOrg();
        String iccard_data = String.valueOf(((PasswordCallback)callbacks[0]).getPassword());
        userName = searchUserAccount(iccard_data,org);
        if(userName != null){
            result = ISAuthConstants.LOGIN_SUCCEED;;
        } else {
            setFailureID(userName);
            throw new AuthLoginException(amAuthICCard, "authFailed", null);

        }

        if (debug.messageEnabled()) {
            debug.message("principal: " + userName);
        }

        return result;
    }


    // cleanup state fields
    public void destroyModuleState() {
        nullifyUsedVars();
    }

    public void nullifyUsedVars() {
        bundle = null;
        sharedState = null;
    }

    /**
    * Searches for an account with user Id userID in the organization organization
    * @param searchAttribute The attribute to be used to search for an identity
    *  in the organization
    * @param attributeValue The attributeValue to compare when searchinf for an
    *  identity in the organization
    * @param organization organization or the organization name where the identity will be
    *  looked up
    * @return the attribute value for the identity searched. Empty string if not found or
    *  null if an error occurs
    */
   @SuppressWarnings({ "unchecked", "rawtypes" })
private String searchUserAccount(String attributeValue, String organization)
           throws AuthLoginException {

       String classMethod = "ICCard.searchUserAccount: ";

       if (organization.isEmpty()) {
           organization = "/";
       }

       if (debug.messageEnabled()) {
           debug.message(classMethod + " searching for user " + attributeValue
                   + " in the organization =" + organization);
       }

       // And the search criteria
       IdSearchControl searchControl = new IdSearchControl();
       searchControl.setMaxResults(1);
       searchControl.setTimeOut(3000);

       searchControl.setSearchModifiers(IdSearchOpModifier.OR, buildSearchControl(attributeValue));
       searchControl.setAllReturnAttributes(false);

       try {
           AMIdentityRepository amirepo = new AMIdentityRepository(getSSOSession(), organization);

           IdSearchResults searchResults = amirepo.searchIdentities(IdType.USER, "*", searchControl);
           if (searchResults.getErrorCode() == IdSearchResults.SUCCESS && searchResults != null) {
               Set<AMIdentity> results = searchResults.getSearchResults();
               if (!results.isEmpty()) {
                   if (debug.messageEnabled()) {
                       debug.message(classMethod + results.size() + " result(s) obtained");
                   }
                   AMIdentity userDNId = results.iterator().next();
                   String user = userDNId.getUniversalId();
                   Set attr = userDNId.getAttribute(authAttribute);
                   if (userDNId != null) {
                       if (debug.messageEnabled()) {
                            debug.message(classMethod + "user = " + user);
                            debug.message(classMethod + "attr =" + attr);
                       }
                       return user;
                   }
               }
           }
       } catch (IdRepoException idrepoex) {
           String data[] = {attributeValue, organization};
           throw new AuthLoginException(amAuthICCard,
               "idRepoSearch", data, idrepoex);
       } catch (SSOException ssoe) {
           String data[] = {attributeValue, organization};
           throw new AuthLoginException(amAuthICCard,
               "ssoSearch", data, ssoe);
       }
       if (debug.messageEnabled()) {
                   debug.message(classMethod + " No results were found !");
       }
       return null;
   }

   private Map<String, Set<String>> buildSearchControl(String value)
           throws AuthLoginException {
       Map<String, Set<String>> attr = new HashMap<String, Set<String>>();
       Set<String> valueSet = new HashSet<String>();
       valueSet.add(value);
       attr.put(authAttribute, valueSet);
       return attr;
   }


}
