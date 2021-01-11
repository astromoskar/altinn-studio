import { SagaIterator } from 'redux-saga';
import { call, takeLatest, select } from 'redux-saga/effects';
import { IAltinnWindow, IInstance } from 'altinn-shared/types';
import { IApplicationMetadata } from 'src/shared/resources/applicationMetadata';
import { get } from '../../../../utils/networking';
import { getRuleModelFields } from '../../../../utils/rules';
import Actions from '../rulesActions';
import * as FetchActions from './fetchRulesActions';
import * as ActionTypes from '../rulesActionTypes';
import QueueActions from '../../../../shared/resources/queue/queueActions';
import { IRuntimeState, ILayoutSets } from '../../../../types';
import { getLayouytsetForDataElement } from '../../../../utils/layout';
import { getDataTaskDataTypeId } from '../../../../utils/appMetadata';

const layoutSetsSelector = (state: IRuntimeState) => state.formLayout.layoutset;
const instanceSelector = (state: IRuntimeState) => state.instanceData.instance;
const applicationMetadataSelector = (state: IRuntimeState) => state.applicationMetadata.applicationMetadata;

/**
 * Saga to retrive the rule configuration defining which rules to run for a given UI
 */
function* fetchRuleModelSaga({
  url,
}: FetchActions.IFetchRuleModel): SagaIterator {
  try {
    const { org, app } = window as Window as IAltinnWindow;
    const layoutSets: ILayoutSets = yield select(layoutSetsSelector);
    const instance: IInstance = yield select(instanceSelector);
    const aplicationMetadataState: IApplicationMetadata = yield select(applicationMetadataSelector);
    const dataType: string =
    getDataTaskDataTypeId(instance.process.currentTask.elementId, aplicationMetadataState.dataTypes);
    let apiUrl: string = url;

    if (layoutSets != null) {
      const layoutSetId: string = getLayouytsetForDataElement(instance, dataType, layoutSets);
      apiUrl = `${window.location.origin}/${org}/${app}/api/rulehandler/${layoutSetId}`;
    }

    const ruleModel = yield call(get, apiUrl);
    const scriptEle = window.document.createElement('script');
    scriptEle.innerHTML = ruleModel;
    window.document.body.appendChild(scriptEle);
    const ruleModelFields = getRuleModelFields();

    yield call(
      Actions.fetchRuleModelFulfilled,
      ruleModelFields,
    );
  } catch (err) {
    yield call(Actions.fetchRuleModelRejected, err);
    yield call(QueueActions.dataTaskQueueError, err);
  }
}

export function* watchFetchRuleModelSaga(): SagaIterator {
  yield takeLatest(ActionTypes.FETCH_RULE_MODEL, fetchRuleModelSaga);
}
