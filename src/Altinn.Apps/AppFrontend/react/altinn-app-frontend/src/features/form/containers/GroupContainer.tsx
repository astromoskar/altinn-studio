/* eslint-disable no-undef */
/* eslint-disable react/no-array-index-key */
/* eslint-disable max-len */
import React from 'react';
import { Grid, makeStyles, createMuiTheme, TableContainer, Table, TableHead, TableRow, TableBody, TableCell, IconButton, useMediaQuery } from '@material-ui/core';
import { AltinnButton } from 'altinn-shared/components';
import altinnAppTheme from 'altinn-shared/theme/altinnAppTheme';
import { useDispatch, useSelector } from 'react-redux';
import { getLanguageFromKey, getTextResourceByKey } from 'altinn-shared/utils';
import { componentHasValidations, repeatingGroupHasValidations } from 'src/utils/validation';
import ErrorPaper from 'src/components/message/ErrorPaper';
import { createRepeatingGroupComponents } from 'src/utils/formLayout';
import { makeGetHidden } from 'src/selectors/getLayoutData';
import { getTextFromAppOrDefault } from 'src/utils/textResource';
import { getTextResource } from 'src/utils/formComponentUtils';
import { ILayout, ILayoutComponent, ILayoutGroup, ISelectionComponentProps } from '../layout';
import { renderGenericComponent, setupGroupComponents } from '../../../utils/layout';
import { FormLayoutActions } from '../layout/formLayoutSlice';
import { IRuntimeState, ITextResource, IRepeatingGroups, IValidations, IOption, Triggers } from '../../../types';
import { IFormData } from '../data/formDataReducer';

export interface IGroupProps {
  id: string;
  container: ILayoutGroup;
  components: (ILayoutComponent | ILayoutGroup)[];
  triggers?: Triggers[];
}

const theme = createMuiTheme(altinnAppTheme);

const useStyles = makeStyles({
  addButton: {
    backgroundColor: theme.altinnPalette.primary.white,
    border: `2px dotted ${theme.altinnPalette.primary.blueMedium}`,
    color: theme.altinnPalette.primary.black,
    fontWeight: 'bold',
    width: '100%',
    '&:hover': {
      cursor: 'pointer',
      borderStyle: 'solid',
    },
    '&:focus': {
      outline: `2px solid ${theme.altinnPalette.primary.blueDark}`,
      border: `2px solid ${theme.altinnPalette.primary.blueDark}`,
      outlineOffset: 0,
    },
  },
  addButtonText: {
    fontWeight: 400,
    fontSize: '1.6rem',
    borderBottom: `2px solid${theme.altinnPalette.primary.blue}`,
  },
  editContainer: {
    display: 'inline-block',
    border: `2px dotted ${theme.altinnPalette.primary.blueMedium}`,
    padding: '12px',
    width: '100%',
  },
  deleteItem: {
    paddingBottom: '0px !important',
  },
  saveItem: {
    paddingTop: '0px !important',
  },
  table: {
    tableLayout: 'fixed',
    marginBottom: '12px',
    wordBreak: 'break-word',
  },
  tableHeader: {
    borderBottom: `2px solid ${theme.altinnPalette.primary.blueMedium}`,
    '& th': {
      fontSize: '1.4rem',
      padding: '0px',
      paddingLeft: '6px',
      '& p': {
        fontWeight: 500,
        fontSize: '1.4rem',
        padding: '0px',
        paddingLeft: '6px',
      },
    },
  },
  tableBody: {
    '& td': {
      borderBottom: `2px dotted ${theme.altinnPalette.primary.blueMedium}`,
      padding: '0px',
      paddingLeft: '6px',
      fontSize: '1.4rem',
      whiteSpace: 'nowrap',
      textOverflow: 'ellipsis',
      overflow: 'hidden',
    },
  },
  tableRowError: {
    backgroundColor: '#F9CAD3;',
  },
  errorIcon: {
    fontSize: '2em',
    minWidth: '0px',
    minHeight: '0px',
    width: 'auto',
  },
  addIcon: {
    transform: 'rotate(45deg)',
    fontSize: '3.4rem',
    marginRight: '0.7rem',
  },
  deleteButton: {
    padding: '0px',
    color: 'black',
  },
  editIcon: {
    paddingLeft: '6px',
  },
  mobileGrid: {
    borderBottom: `2px dotted ${theme.altinnPalette.primary.blueMedium}`,
    paddingLeft: '0.6rem',
    paddingRight: '0.6rem',
  },
  mobileContainer: {
    borderTop: `2px solid ${theme.altinnPalette.primary.blueMedium}`,
    marginBottom: '1.2rem',
  },
  mobileText: {
    fontWeight: 500,
    float: 'left',
    maxWidth: '50%',
    whiteSpace: 'nowrap',
    textOverflow: 'ellipsis',
    overflow: 'hidden',
    '& p': {
      fontWeight: 500,
    },
  },
  mobileValueText: {
    whiteSpace: 'nowrap',
    textOverflow: 'ellipsis',
    overflow: 'hidden',
    maxWidth: '50%',
    minWidth: '50%',
  },
  textContainer: {
    width: '100%',
    whiteSpace: 'nowrap',
    textOverflow: 'ellipsis',
    overflow: 'hidden',
  },
});

export function getHiddenFieldsForGroup(hiddenFields: string[], components: (ILayoutGroup | ILayoutComponent)[]) {
  const result = [];
  hiddenFields.forEach((fieldKey) => {
    const fieldKeyWithoutIndex = fieldKey.replace(/-\d{1,}$/, '');
    if (components.find((component) => component.id === fieldKeyWithoutIndex)) {
      result.push(fieldKey);
    }
  });

  return result;
}

export function GroupContainer({
  id,
  container,
  components,
}: IGroupProps): JSX.Element {
  const classes = useStyles();
  const dispatch = useDispatch();
  const [groupErrors, setGroupErrors] = React.useState<string>(null);
  const renderComponents: ILayoutComponent[] = JSON.parse(JSON.stringify(components));
  const validations: IValidations = useSelector((state: IRuntimeState) => state.formValidations.validations);
  const currentView: string = useSelector((state: IRuntimeState) => state.formLayout.uiConfig.currentView);
  const language: any = useSelector((state: IRuntimeState) => state.language.language);
  const repeatingGroups: IRepeatingGroups = useSelector((state: IRuntimeState) => state.formLayout.uiConfig.repeatingGroups);
  const hiddenFields: string[] = useSelector((state: IRuntimeState) => getHiddenFieldsForGroup(state.formLayout.uiConfig.hiddenFields, components));
  const GetHiddenSelector = makeGetHidden();
  const hidden: boolean = useSelector((state: IRuntimeState) => GetHiddenSelector(state, { id }));
  const formData: IFormData = useSelector((state: IRuntimeState) => state.formData.formData);
  const layout: ILayout = useSelector((state: IRuntimeState) => state.formLayout.layouts[state.formLayout.uiConfig.currentView]);
  const editIndex: number = useSelector((state: IRuntimeState) => state.formLayout.uiConfig.repeatingGroups[id].editIndex);
  const options = useSelector((state: IRuntimeState) => state.optionState.options);
  const textResources: ITextResource[] = useSelector((state: IRuntimeState) => state.textResources.resources);
  const getRepeatingGroupIndex = (containerId: string) => {
    if (repeatingGroups && repeatingGroups[containerId]) {
      return repeatingGroups[containerId].count;
    }
    return -1;
  };
  const repeatingGroupIndex = getRepeatingGroupIndex(id);
  const tableHeaderComponents = container.tableHeaders || container.children || [];
  const mobileView = useMediaQuery('(max-width:992px)'); // breakpoint on altinn-modal
  const componentTitles: string[] = [];
  renderComponents.forEach((component: ILayoutComponent) => {
    const childId = (component as any).baseComponentId || component.id;
    if (tableHeaderComponents.includes(childId)) {
      componentTitles.push(component.textResourceBindings?.title || '');
    }
  });
  const repeatingGroupDeepCopyComponents = createRepeatingGroupComponents(
    container,
    renderComponents,
    repeatingGroupIndex,
    textResources,
    hiddenFields,
  );
  const tableHasErrors = repeatingGroupHasValidations(container, repeatingGroupDeepCopyComponents, validations, currentView, repeatingGroups, layout);

  React.useEffect(() => {
    if (validations && validations[currentView] && validations[currentView][id]) {
      let errorText = '';
      validations[currentView][id].group.errors.forEach((error, index) => {
        errorText += `${index > 0 ? ' ,' : ''}${getTextFromAppOrDefault(error, textResources, language, [], true)}`;
      });
      setGroupErrors(errorText);
    } else {
      setGroupErrors(null);
    }
  }, [validations, currentView, id]);

  const onClickAdd = () => {
    dispatch(FormLayoutActions.updateRepeatingGroups({ layoutElementId: id }));
    dispatch(FormLayoutActions.updateRepeatingGroupsEditIndex({ group: id, index: (repeatingGroupIndex + 1) }));
  };

  const onKeypressAdd = (event: React.KeyboardEvent<HTMLDivElement>) => {
    if (event.key === 'Enter' || event.key === ' ' || event.key === 'Spacebar') {
      onClickAdd();
    }
  };

  const getFormDataForComponent = (component: ILayoutComponent | ILayoutGroup, index: number): string => {
    if (component.type === 'Group' || component.type === 'Header' || component.type === 'Paragraph') {
      return '';
    }
    const dataModelBinding = (component.type === 'AddressComponent') ? component.dataModelBindings?.address : component.dataModelBindings?.simpleBinding;
    const replaced = dataModelBinding.replace(container.dataModelBindings.group, `${container.dataModelBindings.group}[${index}]`);
    if (component.type === 'Dropdown' || component.type === 'RadioButtons') {
      const selectionComponent = component as ISelectionComponentProps;
      let label: string;
      if (selectionComponent?.options) {
        label = selectionComponent.options.find((option: IOption) => option.value === formData[replaced])?.label;
      } else if (selectionComponent.optionsId) {
        label = options[selectionComponent.optionsId]?.find((option: IOption) => option.value === formData[replaced])?.label;
      }
      return getTextResourceByKey(label, textResources) || '';
    }
    if (component.type === 'Checkboxes') {
      const selectionComponent = component as ISelectionComponentProps;
      let label: string = '';
      const data: string = formData[replaced];
      const split = data?.split(',');
      split?.forEach((value: string) => {
        if (selectionComponent?.options) {
          label += getTextResourceByKey(selectionComponent.options.find((option: IOption) => option.value === value)?.label, textResources) || '';
        } else if (selectionComponent.optionsId) {
          label += getTextResourceByKey(options[selectionComponent.optionsId]?.find((option: IOption) => option.value === value)?.label, textResources) || '';
        }
        if (split.indexOf(value) < (split.length - 1)) {
          label += ', ';
        }
      });
      return label;
    }
    return formData[replaced] || '';
  };

  const onClickRemove = (groupIndex: number) => {
    dispatch(FormLayoutActions.updateRepeatingGroupsEditIndex({ group: id, index: -1 }));
    dispatch(FormLayoutActions.updateRepeatingGroups({
      layoutElementId: id,
      remove: true,
      index: groupIndex,
    }));
  };

  const onClickEdit = (groupIndex: number) => {
    if (groupIndex === editIndex) {
      dispatch(FormLayoutActions.updateRepeatingGroupsEditIndex({ group: id, index: -1 }));
    } else {
      dispatch(FormLayoutActions.updateRepeatingGroupsEditIndex({ group: id, index: groupIndex }));
    }
  };

  const onClickSave = () => {
    const validate: boolean = container.triggers?.includes(Triggers.Validation);
    dispatch(FormLayoutActions.updateRepeatingGroupsEditIndex({ group: id, index: -1, validate }));
  };

  const childElementHasErrors = (element: ILayoutGroup | ILayoutComponent, index: number) => {
    if (element.type === 'Group') {
      return childGroupHasErrors(element as ILayoutGroup, index);
    }
    return componentHasValidations(validations, currentView, `${element.id}`);
  };

  const childGroupHasErrors = (childGroup: ILayoutGroup, index: number) => {
    const childGroupCount = repeatingGroups[childGroup.id]?.count;
    const childGroupComponents = layout.filter((childElement) => childGroup.children?.indexOf(childElement.id) > -1);
    const childRenderComponents = setupGroupComponents(childGroupComponents, childGroup.dataModelBindings?.group, index);
    const deepCopyComponents = createRepeatingGroupComponents(
      childGroup,
      childRenderComponents,
      childGroupCount,
      textResources,
      hiddenFields,
    );
    return repeatingGroupHasValidations(childGroup, deepCopyComponents, validations, currentView, repeatingGroups, layout, hiddenFields);
  };

  if (hidden) {
    return null;
  }

  return (
    <Grid
      container={true}
      item={true}
    >
      <Grid
        container={true}
        item={true}
        data-testid={`group-${id}`}
        id={`group-${id}`}
      >
        {!mobileView &&
        <TableContainer component={Grid}>
          <Table className={classes.table}>
            <TableHead className={classes.tableHeader}>
              <TableRow>
                {componentTitles.map((title: string) => (
                  <TableCell align='left' key={title}>
                    {getTextResource(title, textResources)}
                  </TableCell>
                ))}
                <TableCell/>
              </TableRow>
            </TableHead>
            <TableBody className={classes.tableBody}>
              {(repeatingGroupIndex >= 0) && [...Array(repeatingGroupIndex + 1)].map((_x: any, index: number) => {
                const rowHasErrors = repeatingGroupDeepCopyComponents[index].some((component: ILayoutComponent | ILayoutGroup) => {
                  return childElementHasErrors(component, index);
                });
                return (
                  <TableRow className={rowHasErrors ? classes.tableRowError : ''} key={index}>
                    {components.map((component: ILayoutComponent) => {
                      const childId = (component as any).baseComponentId || component.id;
                      if (!tableHeaderComponents.includes(childId)) {
                        return null;
                      }
                      return (
                        <TableCell key={`${component.id} ${index}`}>
                          {getFormDataForComponent(component, index)}
                        </TableCell>
                      );
                    })}
                    <TableCell align='right' key={`delete-${index}`}>
                      <IconButton style={{ color: 'black' }} onClick={() => onClickEdit(index)}>
                        {rowHasErrors ?
                          getLanguageFromKey('general.edit_alt_error', language) :
                          getLanguageFromKey('general.edit_alt', language)}
                        <i className={rowHasErrors ?
                          `ai ai-circle-exclamation a-icon ${classes.errorIcon} ${classes.editIcon}` :
                          `fa fa-editing-file ${classes.editIcon}`}
                        />
                      </IconButton>
                    </TableCell>
                  </TableRow>);
              })}
            </TableBody>
          </Table>
        </TableContainer>}
        {mobileView &&
        <Grid
          container={true}
          item={true}
          direction='column'
          className={classes.mobileContainer}
        >
          {(repeatingGroupIndex >= 0) && [...Array(repeatingGroupIndex + 1)].map((_x: any, index: number) => {
            const rowHasErrors = repeatingGroupDeepCopyComponents[index].some((component: ILayoutComponent | ILayoutGroup) => {
              return childElementHasErrors(component, index);
            });
            return (
              <Grid
                item={true} container={true}
                justify='flex-end' direction='row'
                className={`${classes.mobileGrid} ${rowHasErrors ? classes.tableRowError : ''}`}
              >
                <Grid item={true}>
                  <IconButton
                    style={{
                      color: 'black', padding: '0px', paddingLeft: '6px',
                    }} onClick={() => onClickEdit(index)}
                  >
                    {rowHasErrors ?
                      getLanguageFromKey('general.edit_alt_error', language) :
                      getLanguageFromKey('general.edit_alt', language)}
                    <i className={rowHasErrors ?
                      `ai ai-circle-exclamation ${classes.errorIcon}` :
                      `fa fa-editing-file ${classes.editIcon}`}
                    />
                  </IconButton>
                </Grid>
                {componentTitles.map((title: string, titleIndex: number) => {
                  return (
                    <Grid item={true} className={rowHasErrors ? `${classes.tableRowError} ${classes.textContainer}` : classes.textContainer}>
                      <div className={classes.mobileText}>
                        {getTextResource(title, textResources)}
                      </div>
                      <div
                        className={classes.mobileValueText}
                      >
                        {`: ${getFormDataForComponent(components[titleIndex], titleIndex)}`}
                      </div>
                    </Grid>
                  );
                })}
              </Grid>
            );
          })}
        </Grid>
        }
      </Grid>
      <Grid
        container={true}
        justify='flex-end'
      />
      {((editIndex < 0) && ((repeatingGroupIndex + 1) < container.maxCount)) &&
      <Grid
        container={true}
        direction='row'
        justify='center'
      >
        <Grid
          item={true}
          container={true}
          direction='row'
          xs={12}
          className={classes.addButton}
          role='button'
          tabIndex={0}
          onClick={onClickAdd}
          onKeyPress={(event) => onKeypressAdd(event)}
          justify='center'
          alignItems='center'
        >
          <Grid item={true}>
            <i className={`fa fa-exit ${classes.addIcon}`} />
          </Grid>
          <Grid item={true}>
            <span className={classes.addButtonText}>
              {`${getLanguageFromKey('general.add_new', language)}
              ${container.textResourceBindings?.add_button ? getTextResourceByKey(container.textResourceBindings.add_button, textResources) : ''}`}
            </span>
          </Grid>
        </Grid>
      </Grid>
      }
      {(editIndex >= 0) &&
      <div className={classes.editContainer}>
        <Grid
          container={true}
          item={true}
          direction='row'
          spacing={3}
        >
          <Grid
            item={true}
            container={true}
            direction='column'
            alignItems='flex-end'
            spacing={3}
            className={classes.deleteItem}
          >
            <Grid item={true}>
              <IconButton
                classes={{ root: classes.deleteButton }}
                onClick={() => onClickRemove(editIndex)}
              >
                {getLanguageFromKey('general.delete', language)}
                <i className='ai ai-trash'/>
              </IconButton>
            </Grid>
          </Grid>
          <Grid
            container={true}
            alignItems='flex-start'
            item={true}
            spacing={3}
          >
            { repeatingGroupDeepCopyComponents[editIndex]?.map((component: ILayoutComponent) => { return renderGenericComponent(component, layout, editIndex); }) }
          </Grid>
          <Grid
            item={true}
            spacing={3}
            className={classes.saveItem}
          >
            <AltinnButton
              btnText={getLanguageFromKey('general.save', language)}
              onClickFunction={onClickSave}
              id={`add-button-grp-${id}`}
            />
          </Grid>
        </Grid>
      </div>}
      {tableHasErrors &&
      <Grid
        container={true}
        style={{ paddingTop: '12px' }}
        direction='column'
      >
        <ErrorPaper
          message={getLanguageFromKey('group.row_error', language)}
        />
      </Grid>
      }
      {groupErrors &&
      <Grid
        container={true}
        style={{ paddingTop: '12px' }}
        direction='column'
      >
        <ErrorPaper
          message={groupErrors}
        />
      </Grid>
      }
    </Grid>
  );
}
